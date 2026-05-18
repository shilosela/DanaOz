using DanaOz.DAL.Repositories;
using DanaOz.BLL.Models;

namespace DanaOz.BLL.Services
{
    public class ConversationService
    {
        private readonly UserRepository _userRepository;
        private readonly ChatLogRepository _chatLogRepository;
        private readonly UserPointsRepository _userPointsRepository;
        private readonly OnboardingService _onboardingService;
        private readonly AIService _aiService;

        public ConversationService(
            UserRepository userRepository,
            ChatLogRepository chatLogRepository,
            UserPointsRepository userPointsRepository,
            OnboardingService onboardingService,
            AIService aiService)
        {
            _userRepository = userRepository;
            _chatLogRepository = chatLogRepository;
            _userPointsRepository = userPointsRepository;
            _onboardingService = onboardingService;
            _aiService = aiService;
        }

        public async Task<BotResponseModel> HandleIncomingMessageAsync(MessageModel incomingMessage)
        {
            try
            {
                // Step 1 — Get user
                var user = await _userRepository.GetByPhoneNumberAsync(incomingMessage.PhoneNumber);

                // Step 2 — Log incoming message
                var chatLog = await _chatLogRepository.LogInboundAsync(
                    incomingMessage.PhoneNumber,
                    user?.UserId,
                    incomingMessage.MessageText);

                // Step 3 — New user? Create and start onboarding
                if (user == null)
                {
                    user = await _userRepository.CreateAsync(incomingMessage.PhoneNumber);
                    var welcomeMessage = await _onboardingService.HandleOnboardingAsync(user, incomingMessage.MessageText);
                    await _chatLogRepository.LogOutboundAsync(incomingMessage.PhoneNumber, user.UserId, welcomeMessage!);
                    return new BotResponseModel { PhoneNumber = incomingMessage.PhoneNumber, MessageText = welcomeMessage! };
                }

                // Step 4 — Update last login
                await _userRepository.UpdateLastLoginAsync(user.UserId);

                // Step 5 — Still onboarding?
                if (!user.IsOnboarded)
                {
                    var onboardingResponse = await _onboardingService.HandleOnboardingAsync(user, incomingMessage.MessageText);
                    if (onboardingResponse != null)
                    {
                        await _chatLogRepository.LogOutboundAsync(incomingMessage.PhoneNumber, user.UserId, onboardingResponse);
                        return new BotResponseModel { PhoneNumber = incomingMessage.PhoneNumber, MessageText = onboardingResponse };
                    }
                }

                // Step 6 — Award points and call AI
                await _userRepository.AddPointsAsync(user.UserId, 5);
                await _userPointsRepository.AddAsync(user.UserId, 5, chatLog.ChatLogId);

                var teacherContext = "שם המורה: " + (user.Name ?? "מורה") + "\n" +
                                    (string.IsNullOrEmpty(user.SchoolName) ? "" : "בית ספר: " + user.SchoolName + "\n") +
                                    "נקודות דנה: " + user.DanaPoints + "\n";

                var aiResponse = await _aiService.GenerateResponseAsync(incomingMessage.MessageText, teacherContext);

                // Step 7 — Log and return
                await _chatLogRepository.LogOutboundAsync(incomingMessage.PhoneNumber, user.UserId, aiResponse);

                return new BotResponseModel
                {
                    PhoneNumber = incomingMessage.PhoneNumber,
                    MessageText = aiResponse
                };
            }
            catch (Exception ex)
            {
                return new BotResponseModel
                {
                    PhoneNumber = incomingMessage.PhoneNumber,
                    MessageText = "אוי, סליחה! חוויתי עומס רגעי. אנא נסה שוב בעוד כמה שניות",
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}