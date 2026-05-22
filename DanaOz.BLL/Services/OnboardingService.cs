using DanaOz.DAL.Entities;
using DanaOz.DAL.Repositories;

namespace DanaOz.BLL.Services
{
    public class OnboardingService
    {
        private readonly UserRepository _userRepository;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly UserClassRepository _userClassRepository;
        private readonly UserSchoolRepository _userSchoolRepository;

        private static readonly Dictionary<int, string> SubjectNames = new()
        {
            { 10, "מתמטיקה" },
            { 11, "מדעי המחשב" },
            { 12, "כימיה" },
            { 13, "ביולוגיה" },
            { 14, "מדעים כלליים" },
            { 20, "אנגלית" },
            { 21, "עברית" },
            { 30, "היסטוריה" },
            { 31, "אזרחות" },
            { 32, "גיאוגרפיה" },
        };

        private static readonly Dictionary<int, string> GradeNames = new()
        {
            { 1,  "א'"   }, { 2,  "ב'"   }, { 3,  "ג'"   }, { 4,  "ד'"   },
            { 5,  "ה'"   }, { 6,  "ו'"   }, { 7,  "ז'"   }, { 8,  "ח'"   },
            { 9,  "ט'"   }, { 10, "י'"   }, { 11, "י\"א" }, { 12, "י\"ב" }
        };

        public OnboardingService(
            UserRepository userRepository,
            UserSettingsRepository userSettingsRepository,
            UserClassRepository userClassRepository,
            UserSchoolRepository userSchoolRepository)
        {
            _userRepository = userRepository;
            _userSettingsRepository = userSettingsRepository;
            _userClassRepository = userClassRepository;
            _userSchoolRepository = userSchoolRepository;
        }

        public async Task<string?> HandleOnboardingAsync(User user, string incomingMessage)
        {
            switch (user.OnboardingStep)
            {
                // ── Step 1: Welcome ───────────────────────────────────────────
                case 0:
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 1);
                    return WelcomeMessage();

                // ── Step 1: Collect Name ──────────────────────────────────────
                case 1:
                    user.Name = incomingMessage.Trim();
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 2);
                    return $"נעים מאוד, {user.Name}! 😊\n\n" +
                           "ידעת? 80% מהמורים הם נשים 👩‍🏫\n\n" +
                           "מה המגדר שלך?\n\n" +
                           "1️⃣ זכר\n" +
                           "2️⃣ נקבה\n\n" +
                           "ניתן לכתוב 1 לזכר או 2 לנקבה.";

                // ── Step 2: Collect Gender ────────────────────────────────────
                case 2:
                    user.Gender = ParseGender(incomingMessage);
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 3);
                    return SchoolTypeQuestion();

                // ── Step 3: Collect School Type(s) ───────────────────────────
                case 3:
                    var schoolTypes = ParseNumberList(incomingMessage, 1, 4);
                    if (!schoolTypes.Any())
                        return "לא הצלחתי להבין.\n" +
                               "אנא כתוב/י מספר בין 1 ל-4:\n" +
                               "1️⃣ יסודי  2️⃣ חטיבת ביניים  3️⃣ תיכון  4️⃣ אחר";

                    await _userSettingsRepository.SetAsync(user.UserId, "ob_school_types", string.Join(",", schoolTypes));
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 4);
                    return SubjectQuestion();

                // ── Step 4: Collect Subject(s) ────────────────────────────────
                case 4:
                    var subjects = ParseNumberList(incomingMessage, 10, 43);
                    if (!subjects.Any())
                        return "לא הצלחתי להבין.\n" +
                               "אנא כתוב/י מספר מקצוע, למשל 10 למתמטיקה, 20 לאנגלית.";

                    await _userSettingsRepository.SetAsync(user.UserId, "ob_subjects", string.Join(",", subjects));
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 5);
                    return "הוראה היא לא רק מקצוע.\n" +
                           "יש בה משמעות וערך עמוק. ❤️\n\n" +
                           "עכשיו אני מכיר/ה אותך הרבה יותר טוב 🌟\n\n" +
                           "בואי נלמד עוד על בתי הספר והכיתות שלך...\n\n" +
                           "רוב המורים מלמדים בבית ספר אחד.\n" +
                           "בכמה בתי ספר את/ה מלמד/ת?\n" +
                           "נא לכתוב מספר: 1, 2 או 3.";

                // ── Step 5 (spec Step 6): Collect School Count ────────────────
                case 5:
                    if (!int.TryParse(incomingMessage.Trim(), out int schoolCount) || schoolCount < 1)
                        return "אנא כתוב/י מספר, לדוגמה: 1, 2 או 3.";

                    schoolCount = Math.Min(schoolCount, 4);
                    await _userSettingsRepository.SetAsync(user.UserId, "ob_school_count", schoolCount.ToString());
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 6);

                    if (schoolCount == 1)
                        return "מצוין! 🏫\n\nבאיזה עיר או ישוב נמצא בית הספר?";
                    else
                        return "וואו - ללמד ביותר מבית ספר אחד זה אתגר לא קטן! 💪\n\n" +
                               "באיזה עיר או ישוב נמצא בית הספר הראשון?";

                // ── Step 6: Collect School 1 City ─────────────────────────────
                case 6:
                    user.City = incomingMessage.Trim();
                    await _userRepository.UpdateAsync(user);
                    await _userSettingsRepository.SetAsync(user.UserId, "ob_school_1_city", incomingMessage.Trim());
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 7);
                    return "מה שם בית הספר?";

                // ── Step 7: Collect School 1 Name → create UserSchool record ──
                case 7:
                    var cityForSchool1 = (await _userSettingsRepository.GetAsync(user.UserId, "ob_school_1_city"))?.SettingsValue;
                    await _userSchoolRepository.CreateAsync(new UserSchool
                    {
                        UserId = user.UserId,
                        SchoolName = incomingMessage.Trim(),
                        City = cityForSchool1,
                        SortOrder = 1
                    });

                    var countSetting7 = await _userSettingsRepository.GetAsync(user.UserId, "ob_school_count");
                    int schoolCount7 = int.TryParse(countSetting7?.SettingsValue, out int sc7) ? sc7 : 1;

                    if (schoolCount7 >= 2)
                    {
                        await _userRepository.UpdateOnboardingStepAsync(user.UserId, 8);
                        return "מה שם בית הספר השני?";
                    }

                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 10);
                    return ClassGradeQuestion(incomingMessage.Trim());

                // ── Step 8: Collect School 2 Name → create UserSchool record ──
                case 8:
                    await _userSchoolRepository.CreateAsync(new UserSchool
                    {
                        UserId = user.UserId,
                        SchoolName = incomingMessage.Trim(),
                        SortOrder = 2
                    });

                    var countSetting8 = await _userSettingsRepository.GetAsync(user.UserId, "ob_school_count");
                    int schoolCount8 = int.TryParse(countSetting8?.SettingsValue, out int sc8) ? sc8 : 2;

                    if (schoolCount8 >= 3)
                    {
                        await _userRepository.UpdateOnboardingStepAsync(user.UserId, 9);
                        return "מה שם בית הספר השלישי?";
                    }

                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 10);
                    var s1for8 = (await _userSchoolRepository.GetFirstByUserIdAsync(user.UserId))?.SchoolName ?? "בית הספר";
                    return ClassGradeQuestion(s1for8);

                // ── Step 9: Collect School 3 Name → create UserSchool record ──
                case 9:
                    await _userSchoolRepository.CreateAsync(new UserSchool
                    {
                        UserId = user.UserId,
                        SchoolName = incomingMessage.Trim(),
                        SortOrder = 3
                    });

                    var countSetting9 = await _userSettingsRepository.GetAsync(user.UserId, "ob_school_count");
                    int schoolCount9 = int.TryParse(countSetting9?.SettingsValue, out int sc9) ? sc9 : 3;

                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 10);
                    var s1for9 = (await _userSchoolRepository.GetFirstByUserIdAsync(user.UserId))?.SchoolName ?? "בית הספר";

                    string capPrefix = schoolCount9 >= 4 ? "3 בתי ספר זה יותר מספיק להתחלה! 😊\n\n" : "";
                    return capPrefix + ClassGradeQuestion(s1for9);

                // ── Step 7 (spec): Collect Class Grade ────────────────────────
                case 10:
                    if (!int.TryParse(incomingMessage.Trim(), out int grade) || grade < 1 || grade > 12)
                        return "אנא כתוב/י מספר בין 1 ל-12 (1 לכיתה א', 12 לכיתה י\"ב).";

                    await _userSettingsRepository.SetAsync(user.UserId, "ob_class_grade", grade.ToString());

                    var subjSetting10 = await _userSettingsRepository.GetAsync(user.UserId, "ob_subjects");
                    var subjectCodes10 = ParseNumberList(subjSetting10?.SettingsValue ?? "", 10, 43);

                    if (subjectCodes10.Count > 1)
                    {
                        await _userRepository.UpdateOnboardingStepAsync(user.UserId, 11);
                        return SubjectChoiceQuestion(subjectCodes10);
                    }

                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 12);
                    return ClassNameQuestion(grade, subjectCodes10.FirstOrDefault());

                // ── Step 7 (spec): Collect Class Subject (multi-subject only) ─
                case 11:
                    var chosen = ParseNumberList(incomingMessage, 10, 43);
                    if (!chosen.Any())
                        return "אנא כתוב/י את מספר המקצוע מהרשימה.";

                    await _userSettingsRepository.SetAsync(user.UserId, "ob_class_subject", chosen.First().ToString());
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 12);

                    var gradeSetting11 = await _userSettingsRepository.GetAsync(user.UserId, "ob_class_grade");
                    int grade11 = int.TryParse(gradeSetting11?.SettingsValue, out int g11) ? g11 : 0;
                    return ClassNameQuestion(grade11, chosen.First());

                // ── Step 7 (spec): Collect Class Name → Complete ──────────────
                case 12:
                    var gradeSetting12 = await _userSettingsRepository.GetAsync(user.UserId, "ob_class_grade");
                    int grade12 = int.TryParse(gradeSetting12?.SettingsValue, out int g12) ? g12 : 0;
                    string hebrewGrade = GradeNames.GetValueOrDefault(grade12, grade12.ToString());

                    await _userClassRepository.CreateAsync(new UserClass
                    {
                        UserId = user.UserId,
                        Grade = hebrewGrade,
                        ClassName = incomingMessage.Trim()
                    });

                    user.IsOnboarded = true;
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 99);
                    await _userRepository.AddPointsAsync(user.UserId, 50);

                    return $"🎉 הכל מוכן! ברוכ/ה הבא/ה למשפחת דנה עוז!\n\n" +
                           $"קיבלת 50 נקודות דנה על ההרשמה! ⭐\n\n" +
                           $"פשוט כתוב/י לי מה את/ה צריכ/ה, למשל:\n" +
                           $"🔹 \"תכיני לי דף תרגול לכיתה {hebrewGrade}\"\n" +
                           $"🔹 \"תזכירי לי שבוע לפני המבחן\"";

                default:
                    return null;
            }
        }

        public bool IsOnboardingComplete(User user) => user.IsOnboarded;

        private static string WelcomeMessage() =>
            "היי! 👋\n\n" +
            "שמי דנה עוז. 🌟\n\n" +
            "אני אהיה העוזרת האישית שלך.\n\n" +
            "בואו נכיר קצת...\n\n" +
            "מה שמך?";

        private static string SchoolTypeQuestion() =>
            "מצוין! 📚\n\n" +
            "באיזה סוג בית ספר את/ה מלמד/ת?\n\n" +
            "1️⃣ יסודי\n" +
            "2️⃣ חטיבת ביניים\n" +
            "3️⃣ תיכון\n" +
            "4️⃣ אחר (אוניברסיטה / מכללה / גן ילדים)\n\n" +
            "כתוב/י את סוג בית הספר או רק מספר (1 ליסודי, 2 לחטיבה וכו').\n" +
            "אם את/ה מלמד/ת ביותר מסוג אחד, ניתן לכתוב כמה מספרים.\n" +
            "למשל: \"2 3\" לחטיבת ביניים ותיכון.";

        private static string SubjectQuestion() =>
            "איזה מקצוע/ות את/ה מלמד/ת?\n\n" +
            "🔬 *מדעים*\n" +
            "10 - מתמטיקה\n" +
            "11 - מדעי המחשב\n" +
            "12 - כימיה\n" +
            "13 - ביולוגיה\n" +
            "14 - מדעים כלליים\n\n" +
            "🌍 *שפות ותקשורת*\n" +
            "20 - אנגלית\n" +
            "21 - עברית\n\n" +
            "📜 *מדעי הרוח והחברה*\n" +
            "30 - היסטוריה\n" +
            "31 - אזרחות\n" +
            "32 - גיאוגרפיה\n\n" +
            "אם המקצוע שלך לא מופיע ברשימה, פשוט כתוב/י את שמו ושלח/י אלינו.\n\n" +
            "אם את/ה מלמד/ת יותר ממקצוע אחד, ניתן לכתוב את המספרים ברצף.\n" +
            "למשל: \"20 21\" לאנגלית ועברית.";

        private static string ClassGradeQuestion(string schoolName) =>
            $"בואי נתחיל עם הכיתה הראשונה שאת/ה מלמד/ת ב{schoolName}. 📖\n\n" +
            "מה מספר הכיתה?\n\n" +
            "1 - כיתה א'\n" +
            "2 - כיתה ב'\n" +
            "3 - כיתה ג'\n" +
            "4 - כיתה ד'\n" +
            "5 - כיתה ה'\n" +
            "6 - כיתה ו'\n" +
            "7 - כיתה ז'\n" +
            "8 - כיתה ח'\n" +
            "9 - כיתה ט'\n" +
            "10 - כיתה י'\n" +
            "11 - כיתה י\"א\n" +
            "12 - כיתה י\"ב\n\n" +
            "כתוב/י את מספר הכיתה.";

        private static string SubjectChoiceQuestion(List<int> codes)
        {
            var sb = new System.Text.StringBuilder("איזה מקצוע את/ה מלמד/ת בכיתה זו?\n\n");
            foreach (var code in codes)
            {
                if (SubjectNames.TryGetValue(code, out var name))
                    sb.AppendLine($"{code} - {name}");
            }
            sb.Append("\nכתוב/י את מספר המקצוע.");
            return sb.ToString();
        }

        private static string ClassNameQuestion(int grade, int subjectCode)
        {
            string gradeName = GradeNames.GetValueOrDefault(grade, grade.ToString());
            string subjectName = SubjectNames.GetValueOrDefault(subjectCode, "");
            string suggestion = string.IsNullOrEmpty(subjectName)
                ? $"כיתה {gradeName}"
                : $"כיתה {gradeName} {subjectName}";
            return $"איך תרצה/י לקרוא לכיתה זו? 🏷️\n\n" +
                   $"למשל: \"{suggestion}\" או \"קבוצה 5 - כיתה {gradeName}\"";
        }

        private static string ParseGender(string input)
        {
            var v = input.Trim().ToLower();
            return v is "1" or "male" or "m" or "זכר" or "ז" ? "male" : "female";
        }

        private static List<int> ParseNumberList(string input, int min, int max)
        {
            var result = new List<int>();
            if (string.IsNullOrWhiteSpace(input)) return result;
            foreach (var token in input.Split(new[] { ' ', ',', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(token, out int val) && val >= min && val <= max && !result.Contains(val))
                    result.Add(val);
            }
            return result;
        }
    }
}
