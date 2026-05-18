using DanaOz.DAL.Entities;
using DanaOz.DAL.Repositories;

namespace DanaOz.BLL.Services
{
    public class OnboardingService
    {
        private readonly UserRepository _userRepository;
        private readonly UserSettingsRepository _userSettingsRepository;

        public OnboardingService(
            UserRepository userRepository,
            UserSettingsRepository userSettingsRepository)
        {
            _userRepository = userRepository;
            _userSettingsRepository = userSettingsRepository;
        }

        public async Task<string?> HandleOnboardingAsync(User user, string incomingMessage)
        {
            switch (user.OnboardingStep)
            {
                case 0:
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 1);
                    return GetWelcomeMessage();

                case 1:
                    user.Name = incomingMessage.Trim();
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 2);
                    return "נעים מאוד " + user.Name + "! 🌟\n\n" +
                           "באיזה בית ספר את/ה מלמד/ת ואילו מקצועות וכיתות?\n" +
                           "(למשל: מוסינזון הוד השרון, מתמטיקה כיתה י' 3 יחידות)";

                case 2:
                    user.SchoolName = incomingMessage.Trim();
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 3);
                    return "מעולה, רשמתי! 📝\n\n" +
                           "במה תרצה/י שאעזור לך? (אפשר לבחור כמה)\n\n" +
                           "1️⃣ חומרי הוראה (מערכי שיעור, דפי תרגול, מבחנים)\n" +
                           "2️⃣ תזכורות חכמות\n" +
                           "3️⃣ עזרה בבדיקת עבודות";

                case 3:
                    await _userSettingsRepository.SetAsync(user.UserId, "preferences", incomingMessage.Trim());
                    user.IsOnboarded = true;
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.UpdateOnboardingStepAsync(user.UserId, 4);
                    await _userRepository.AddPointsAsync(user.UserId, 50);
                    return "🎉 הכל מוכן! ברוכ/ה הבא/ה למשפחת דנה עוז!\n\n" +
                           "קיבלת 50 נקודות דנה על ההרשמה! ⭐\n\n" +
                           "פשוט כתוב/י לי מה את/ה צריכ/ה, למשל:\n" +
                           "🔹 \"תכיני לי דף תרגול באחוזים לכיתה י'\"\n" +
                           "🔹 \"תזכירי לי שבוע לפני המבחן\"";

                default:
                    return null;
            }
        }

        public bool IsOnboardingComplete(User user)
        {
            return user.IsOnboarded;
        }

        private string GetWelcomeMessage()
        {
            return "היי! 👋 איזה כיף שהגעת אלי!\n\n" +
                   "אני דנה עוז, העוזרת האישית החכמה שלך 🌟\n\n" +
                   "אני כאן כדי לעזור לך בהכנת חומרי לימוד, מערכי שיעור, מבחנים ותזכורות.\n\n" +
                   "איך קוראים לך?";
        }
    }
}