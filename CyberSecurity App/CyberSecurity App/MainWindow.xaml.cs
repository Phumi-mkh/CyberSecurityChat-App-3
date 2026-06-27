
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1;

namespace CyberSecurity_App
{
    public partial class MainWindow : Window
    {
        // ==================== EXISTING FEATURES ====================
        private SpeechSynthesizer? synthesizer;
        private bool isVoiceEnabled = false;
        private bool hasUserName = false;
        private string userName = "";
        private string currentTopic = "";
        private Random random = new Random();
        private Dictionary<string, string> userMemory = new Dictionary<string, string>();

        // ==================== PART 3: TASK STORAGE (In-Memory) ====================
        private List<TaskItem> tasks = new List<TaskItem>();
        private int nextTaskId = 1;

        // ==================== PART 3: NLP SIMULATION ====================
        private Dictionary<string, List<string>> nlpPatterns = new Dictionary<string, List<string>>()
        {
            { "add_task", new List<string> { "add task", "create task", "new task", "add a task", "create a task" } },
            { "set_reminder", new List<string> { "remind me", "set reminder", "reminder for", "set a reminder" } },
            { "show_tasks", new List<string> { "show tasks", "view tasks", "list tasks", "my tasks", "what are my tasks" } },
            { "complete_task", new List<string> { "complete task", "done task", "mark complete", "finish task" } },
            { "delete_task", new List<string> { "delete task", "remove task", "clear task" } },
            { "show_log", new List<string> { "show log", "activity log", "what have you done", "show activity", "recent actions" } },
            { "start_quiz", new List<string> { "start quiz", "begin quiz", "take quiz", "cybersecurity quiz", "test me" } },
            { "show_help", new List<string> { "help", "what can you do", "commands", "options" } }
        };

        // ==================== PART 3: QUIZ SYSTEM ====================
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private int currentQuestionIndex = 0;
        private int quizScore = 0;
        private bool isQuizActive = false;

        // ==================== PART 3: ACTIVITY LOG ====================
        private List<ActivityLogEntry> activityLog = new List<ActivityLogEntry>();
        private int maxLogEntries = 10;

        // ==================== BOT RESPONSES ====================
        private Dictionary<string, string[]> responses = new Dictionary<string, string[]>()
        {
            { "password", new string[] {
                "🔐 Use strong passwords with uppercase, lowercase, numbers, and symbols!",
                "🔑 Never reuse passwords across different websites!",
                "💡 Create passwords that are at least 12 characters long.",
                "⚠️ Enable two-factor authentication for extra protection!"
            }},
            { "phishing", new string[] {
                "🎣 Never click suspicious links in emails or messages!",
                "📧 Always check the sender's email address carefully.",
                "🔍 Hover over links to see the real URL before clicking.",
                "⚠️ Legitimate companies never ask for passwords via email!"
            }},
            { "malware", new string[] {
                "🦠 Keep your antivirus software updated at all times!",
                "💿 Don't download attachments from unknown senders.",
                "🚫 Avoid clicking on pop-up ads or suspicious links.",
                "🔄 Back up your important files regularly!"
            }},
            { "scam", new string[] {
                "🚨 Never trust calls asking for personal information!",
                "⚠️ Scammers create urgency - 'Your account will be closed!'",
                "💰 If it sounds too good to be true, it's a scam!",
                "📞 Hang up and call official numbers directly."
            }},
            { "privacy", new string[] {
                "🛡️ Don't share personal information publicly online!",
                "📱 Review app permissions on your phone regularly.",
                "🔒 Use privacy settings on all social media accounts.",
                "🌐 Use a VPN when on public Wi-Fi networks."
            }},
            { "2fa", new string[] {
                "🔐 Two-factor authentication adds an extra security layer!",
                "📱 Use authenticator apps instead of SMS when possible.",
                "✅ Enable 2FA on email, banking, and social media.",
                "🔑 Save backup codes in a safe place!"
            }},
            { "safe browsing", new string[] {
                "🌐 Look for 'https://' and the padlock icon!",
                "🔒 Avoid using public Wi-Fi for banking.",
                "📱 Keep your browser updated to the latest version.",
                "🛡️ Use ad-blockers for safer browsing."
            }}
        };

        private string[] topics = { "password", "phishing", "malware", "scam", "privacy", "2fa", "safe browsing" };
        private object messageBox;

        // ==================== CONSTRUCTOR ====================
        public MainWindow()
        {
            InitializeComponent();
            InitializeSpeechSafely();
            InitializeQuiz();
            ShowWelcomeMessage();
            LogActivity("System", "Bot started");
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        // ==================== INITIALIZATION ====================
        private void InitializeSpeechSafely()
        {
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    synthesizer = new SpeechSynthesizer();
                    isVoiceEnabled = true;
                    btnVoice.Content = "🔊";
                }
                else
                {
                    isVoiceEnabled = false;
                    btnVoice.Content = "🔇";
                }
            }
            catch (Exception)
            {
                isVoiceEnabled = false;
                btnVoice.Content = "🔇";
                synthesizer = null;
            }
        }

        // ==================== QUIZ INITIALIZATION ====================
        private void InitializeQuiz()
        {
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectAnswer = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others from falling victim."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a strong password?",
                    Options = new List<string> { "password123", "12345678", "Sunset$2024!Beach", "qwerty" },
                    CorrectAnswer = 2,
                    Explanation = "A strong password uses uppercase, lowercase, numbers, and symbols, and is at least 12 characters long."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should use the same password for all your online accounts.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Using the same password for all accounts means if one is compromised, all your accounts are at risk."
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string> { "A password manager", "An extra security step beyond passwords", "A type of malware", "A web browser" },
                    CorrectAnswer = 1,
                    Explanation = "2FA adds an extra layer of security by requiring a second verification method like a code from your phone."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi is safe for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Public Wi-Fi networks are often unsecured, making it risky to perform sensitive activities like banking."
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new List<string> { "A fishing technique", "A type of malware", "A scam using fake emails to steal information", "A social media platform" },
                    CorrectAnswer = 2,
                    Explanation = "Phishing uses deceptive emails or messages to trick people into revealing personal or financial information."
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus software is only needed if you download files frequently.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Antivirus protects against various threats, including those that can infect your system even through regular browsing."
                },
                new QuizQuestion
                {
                    Question = "What should you do if you notice suspicious activity on your account?",
                    Options = new List<string> { "Ignore it", "Change your password immediately", "Share it on social media", "Wait for it to stop" },
                    CorrectAnswer = 1,
                    Explanation = "Immediately changing your password can prevent unauthorized access and protect your account."
                },
                new QuizQuestion
                {
                    Question = "True or False: It's safe to click on links from unknown senders.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Links from unknown senders may lead to phishing sites or download malware to your device."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering in cybersecurity?",
                    Options = new List<string> { "Building social networks", "Manipulating people to reveal confidential information", "Creating social media accounts", "Using social media for business" },
                    CorrectAnswer = 1,
                    Explanation = "Social engineering uses psychological manipulation to trick people into giving up sensitive information."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should share your passwords with trusted friends.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "You should never share your passwords with anyone. Use password managers to securely share access when needed."
                },
                new QuizQuestion
                {
                    Question = "What does 'HTTPS' stand for?",
                    Options = new List<string> { "Hyper Text Transfer Protocol Secure", "Hyper Transfer Protocol System", "High Traffic Transfer System", "Hyper Text System" },
                    CorrectAnswer = 0,
                    Explanation = "HTTPS indicates a secure connection, encrypting data between your browser and the website."
                }
            };

            LogActivity("Quiz", "Initialized 12 cybersecurity questions");
        }

        // ==================== ACTIVITY LOG METHODS ====================
        private void LogActivity(string category, string description)
        {
            var entry = new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Category = category,
                Description = description
            };

            activityLog.Insert(0, entry);
            if (activityLog.Count > 50)
            {
                activityLog.RemoveAt(activityLog.Count - 1);
            }
        }

        private string GetActivityLog()
        {
            if (activityLog.Count == 0)
            {
                return "📋 No activities recorded yet. Start using the bot to log actions!";
            }

            string log = "📋 Here's a summary of recent actions:\n\n";
            int count = Math.Min(activityLog.Count, maxLogEntries);

            for (int i = 0; i < count; i++)
            {
                var entry = activityLog[i];
                log += $"{i + 1}. [{entry.Timestamp:HH:mm:ss}] {entry.Category}: {entry.Description}\n";
            }

            if (activityLog.Count > maxLogEntries)
            {
                log += $"\n... and {activityLog.Count - maxLogEntries} more actions. Type 'show more log' to see all.";
            }

            return log;
        }

        private string GetFullActivityLog()
        {
            if (activityLog.Count == 0)
            {
                return "📋 No activities recorded yet.";
            }

            string log = "📋 Full Activity Log:\n\n";
            for (int i = 0; i < activityLog.Count; i++)
            {
                var entry = activityLog[i];
                log += $"{i + 1}. [{entry.Timestamp:HH:mm:ss}] {entry.Category}: {entry.Description}\n";
            }

            return log;
        }

        // ==================== TASK MANAGEMENT (In-Memory) ====================
        private string AddTask(string title, string description = "", DateTime? reminderDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "⚠️ Task title cannot be empty.";

            var task = new TaskItem
            {
                Id = nextTaskId++,
                Title = title,
                Description = string.IsNullOrEmpty(description) ? title : description,
                ReminderDate = reminderDate,
                IsCompleted = false,
                CreatedDate = DateTime.Now
            };

            tasks.Add(task);

            string reminderMsg = reminderDate.HasValue ? $" with reminder on {reminderDate.Value:MMM dd, yyyy} at {reminderDate.Value:HH:mm}" : "";
            LogActivity("Task", $"Added task: {title}");
            return $"✅ Task added: '{title}'{reminderMsg}.\nType 'show tasks' to see all your tasks.";
        }

        private string GetTasksList()
        {
            if (tasks.Count == 0)
                return "📋 You have no tasks. Use 'add task' to create one!";

            string result = "📋 Your Tasks:\n\n";
            int index = 1;

            foreach (var task in tasks)
            {
                string status = task.IsCompleted ? "✅ COMPLETED" : "⏳ PENDING";
                string reminder = task.ReminderDate.HasValue ?
                    $" 🔔 Reminder: {task.ReminderDate.Value:MMM dd, yyyy HH:mm}" : "";

                result += $"{index}. {task.Title} [{status}]{reminder}\n";
                if (!string.IsNullOrEmpty(task.Description))
                    result += $"   📝 {task.Description}\n";
                result += "\n";
                index++;
            }

            return result;
        }

        private string CompleteTask(string input)
        {
            var words = input.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int taskId))
                {
                    var task = tasks.Find(t => t.Id == taskId);
                    if (task != null)
                    {
                        task.IsCompleted = true;
                        LogActivity("Task", $"Completed: {task.Title}");
                        return $"✅ Task '{task.Title}' marked as completed!";
                    }
                    return "⚠️ Task ID not found. Type 'show tasks' to see task IDs.";
                }

                foreach (var task in tasks)
                {
                    if (task.Title.ToLower().Contains(word.ToLower()))
                    {
                        task.IsCompleted = true;
                        LogActivity("Task", $"Completed: {task.Title}");
                        return $"✅ Task '{task.Title}' marked as completed!";
                    }
                }
            }

            return "⚠️ Task not found. Type 'show tasks' to see your tasks.";
        }

        private string DeleteTask(string input)
        {
            var words = input.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int taskId))
                {
                    var task = tasks.Find(t => t.Id == taskId);
                    if (task != null)
                    {
                        string title = task.Title;
                        tasks.Remove(task);
                        LogActivity("Task", $"Deleted: {title}");
                        return $"🗑️ Task '{title}' has been deleted.";
                    }
                    return "⚠️ Task ID not found. Type 'show tasks' to see task IDs.";
                }

                foreach (var task in tasks)
                {
                    if (task.Title.ToLower().Contains(word.ToLower()))
                    {
                        string title = task.Title;
                        tasks.Remove(task);
                        LogActivity("Task", $"Deleted: {title}");
                        return $"🗑️ Task '{title}' has been deleted.";
                    }
                }
            }

            return "⚠️ Task not found. Type 'show tasks' to see your tasks.";
        }

        // ==================== QUIZ METHODS ====================
        private void StartQuiz()
        {
            if (isQuizActive)
            {
                AddBotMessage("⚠️ You're already taking the quiz!");
                return;
            }

            currentQuestionIndex = 0;
            quizScore = 0;
            isQuizActive = true;
            LogActivity("Quiz", "Started quiz");

            AddBotMessage("🎯 Welcome to the Cybersecurity Quiz!");
            AddBotMessage("📋 Answer the following questions. Type 'quit quiz' to exit.");
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            var question = quizQuestions[currentQuestionIndex];
            string message = $"📝 Question {currentQuestionIndex + 1}/{quizQuestions.Count}: {question.Question}\n\n";

            for (int i = 0; i < question.Options.Count; i++)
            {
                string prefix = ((char)('A' + i)).ToString();
                message += $"{prefix}) {question.Options[i]}\n";
            }

            message += $"\n💡 Type the letter (A, B, C, D) or the answer text.";
            AddBotMessage(message);
            LogActivity("Quiz", $"Question {currentQuestionIndex + 1} presented");
        }

        private void ProcessQuizAnswer(string input)
        {
            if (!isQuizActive) return;

            string answer = input.Trim().ToUpper();
            var question = quizQuestions[currentQuestionIndex];

            int selectedIndex = -1;

            if (answer.Length == 1 && answer[0] >= 'A' && answer[0] <= 'D')
            {
                selectedIndex = answer[0] - 'A';
            }
            else
            {
                for (int i = 0; i < question.Options.Count; i++)
                {
                    if (question.Options[i].ToUpper().Contains(answer) || answer.Contains(question.Options[i].ToUpper()))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            if (selectedIndex == -1 || selectedIndex >= question.Options.Count)
            {
                AddBotMessage("⚠️ Invalid answer. Please type A, B, C, or D.");
                return;
            }

            currentQuestionIndex++;
            bool isCorrect = selectedIndex == question.CorrectAnswer;

            if (isCorrect)
            {
                quizScore++;
                AddBotMessage($"✅ Correct! {question.Explanation}");
                LogActivity("Quiz", $"Question {currentQuestionIndex}: Correct");
            }
            else
            {
                string correctText = question.Options[question.CorrectAnswer];
                AddBotMessage($"❌ Incorrect. The correct answer was: {correctText}\n\n{question.Explanation}");
                LogActivity("Quiz", $"Question {currentQuestionIndex}: Incorrect");
            }

            if (currentQuestionIndex >= quizQuestions.Count)
            {
                EndQuiz();
            }
            else
            {
                ShowNextQuestion();
            }
        }

        private void EndQuiz()
        {
            isQuizActive = false;
            int total = quizQuestions.Count;
            double percentage = (double)quizScore / total * 100;

            string feedback;
            if (percentage >= 80)
                feedback = "🌟 Outstanding! You're a cybersecurity pro!";
            else if (percentage >= 60)
                feedback = "👍 Good job! Keep learning to become a cybersecurity expert!";
            else if (percentage >= 40)
                feedback = "📚 Not bad! Review some topics and try again to improve your score!";
            else
                feedback = "💪 Keep learning! Cybersecurity is important - try again to improve your knowledge!";

            AddBotMessage($"🎯 Quiz Complete!\n\n📊 Score: {quizScore}/{total} ({percentage:F0}%)\n\n{feedback}");
            LogActivity("Quiz", $"Finished with score {quizScore}/{total} ({percentage:F0}%)");
        }

        // ==================== NLP SIMULATION ====================
        private string GetNLPIntent(string input)
        {
            string lowerInput = input.ToLower();

            foreach (var pattern in nlpPatterns)
            {
                foreach (var keyword in pattern.Value)
                {
                    if (lowerInput.Contains(keyword))
                    {
                        return pattern.Key;
                    }
                }
            }

            return "unknown";
        }

        private string ExtractTaskDetails(string input)
        {
            string[] prefixes = { "add task", "create task", "new task", "add a task", "create a task", "remind me", "set reminder" };
            string result = input;

            foreach (var prefix in prefixes)
            {
                if (result.ToLower().StartsWith(prefix))
                {
                    result = result.Substring(prefix.Length).Trim();
                    break;
                }
            }

            if (result.ToLower().StartsWith("to "))
            {
                result = result.Substring(3).Trim();
            }

            if (result.StartsWith("- "))
            {
                result = result.Substring(2).Trim();
            }

            return result;
        }

        private DateTime? ExtractReminderDate(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("tomorrow"))
            {
                return DateTime.Now.AddDays(1);
            }

            var words = lower.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "in" && i + 1 < words.Length)
                {
                    if (int.TryParse(words[i + 1], out int days))
                    {
                        return DateTime.Now.AddDays(days);
                    }
                }
            }

            try
            {
                var dateMatch = System.Text.RegularExpressions.Regex.Match(input, @"\d{1,2}/\d{1,2}/\d{4}|\d{4}-\d{2}-\d{2}");
                if (dateMatch.Success)
                {
                    return DateTime.Parse(dateMatch.Value);
                }
            }
            catch { }

            return null;
        }

        // ==================== UI METHODS ====================
        private void ShowWelcomeMessage()
        {
            string asciiArt = @"
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                                                                                      ║
║     ██████╗ ██╗   ██╗██████╗ ███████╗██████╗     ███████╗███████╗██╗   ██╗██╗  ██╗   ║
║    ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗    ██╔════╝██╔════╝╚██╗ ██╔╝██║  ██║   ║
║    ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝    ███████╗█████╗   ╚████╔╝ ███████║   ║
║    ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗    ╚════██║██╔══╝    ╚██╔╝  ██╔══██║   ║
║    ╚██████╗   ██║   ██████╔╝███████╗██║  ██║    ███████║███████╗   ██║   ██║  ██║   ║
║     ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝    ╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═╝   ║
║                                                                                      ║
║                    🔒   C Y B E R   S E C U R I T Y   B O T   🔒                     ║
║                                                                                      ║
║                        'Your Digital Safety Guardian'                                ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝";

            AddSystemMessage(asciiArt);

            string welcome = "👋 Hello! I'm your Cybersecurity Assistant!\n\nI can help you with:\n• Password safety\n• Phishing detection\n• Malware protection\n• Scam detection\n• Privacy protection\n• Two-factor authentication\n• Safe browsing\n\n📋 New Features:\n• Task Management\n• Cybersecurity Quiz\n• Activity Log\n\nWhat's your name?";
            AddBotMessage(welcome);
        }

        private void AddSystemMessage(string message)
        {
            _ = Dispatcher.Invoke(() =>
            {
                var textBlock = new TextBlock
                {
                    Text = message,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255)),
                    FontSize = 11,
                    FontFamily = new FontFamily("Consolas"),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(5)
                };
                chatPanel.Children.Add(textBlock);
                ScrollToBottom();
            });
        }

        private void AddBotMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var bubble = CreateMessageBubble(message, false);
                chatPanel.Children.Add(bubble);
                ScrollToBottom();
                SaveToHistory("Bot", message);

                if (isVoiceEnabled && synthesizer != null)
                {
                    try
                    {
                        synthesizer.SpeakAsync(message);
                    }
                    catch
                    {
                        isVoiceEnabled = false;
                        btnVoice.Content = "🔇";
                    }
                }
            });
        }

        private void AddUserMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var bubble = CreateMessageBubble(message, true);
                chatPanel.Children.Add(bubble);
                ScrollToBottom();
                SaveToHistory("User", message);
            });
        }

        private Border CreateMessageBubble(string text, bool isUser)
        {
            var outerBorder = new Border
            {
                Margin = new Thickness(isUser ? 40 : 10, 5, isUser ? 10 : 40, 5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                MaxWidth = 280
            };

            var innerBorder = new Border
            {
                CornerRadius = new CornerRadius(15),
                Background = isUser ? new SolidColorBrush(Color.FromRgb(220, 248, 198)) : new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Padding = new Thickness(12, 8, 12, 8)
            };

            var stack = new StackPanel();

            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(17, 27, 33)),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            var timeBlock = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(102, 119, 129)),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 5, 0, 0)
            };

            stack.Children.Add(textBlock);
            stack.Children.Add(timeBlock);
            innerBorder.Child = stack;
            outerBorder.Child = innerBorder;

            return outerBorder;
        }

        private void AddTypingIndicator()
        {
            Dispatcher.Invoke(() =>
            {
                var outerBorder = new Border
                {
                    Margin = new Thickness(10, 5, 40, 5),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                var innerBorder = new Border
                {
                    CornerRadius = new CornerRadius(15),
                    Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    Padding = new Thickness(12, 8, 12, 8)
                };

                var textBlock = new TextBlock
                {
                    Text = "Bot is typing...",
                    Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                    FontSize = 12,
                    FontStyle = FontStyles.Italic
                };

                innerBorder.Child = textBlock;
                outerBorder.Child = innerBorder;
                outerBorder.Tag = "typing";
                chatPanel.Children.Add(outerBorder);
                ScrollToBottom();
            });
        }

        private void RemoveTypingIndicator()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = chatPanel.Children.Count - 1; i >= 0; i--)
                {
                    var item = chatPanel.Children[i] as Border;
                    if (item != null && item.Tag?.ToString() == "typing")
                    {
                        chatPanel.Children.RemoveAt(i);
                    }
                }
            });
        }

        private async Task ShowTypingAndProcess(Func<Task> action)
        {
            AddTypingIndicator();
            await Task.Delay(800);
            RemoveTypingIndicator();
            await action();
        }

        // ==================== MAIN PROCESSING METHOD ====================
        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserMessage();
        }

        private async void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ProcessUserMessage();
            }
        }

        private void QuickAction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string command = button?.Tag?.ToString() ?? "";
            if (!string.IsNullOrEmpty(command))
            {
                messageBox.Text = command;
                _ = ProcessUserMessage();
            }
        }

        private async Task ProcessUserMessage()
        {
            string message = messageBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
                return;

            AddUserMessage(message);
            messageBox.Clear();

            await ShowTypingAndProcess(() =>
            {
                ProcessResponse(message);
                return Task.CompletedTask;
            });
        }

        private void ProcessResponse(string message)
        {
            string lowerMsg = message.ToLower();

            // ===== HANDLE QUIZ MODE =====
            if (isQuizActive)
            {
                if (lowerMsg.Contains("quit quiz") || lowerMsg.Contains("exit quiz"))
                {
                    isQuizActive = false;
                    AddBotMessage("👋 Quiz ended. You can start a new quiz anytime with 'start quiz'.");
                    LogActivity("Quiz", "User quit quiz early");
                    return;
                }

                ProcessQuizAnswer(message);
                return;
            }

            // ===== HANDLE EXIT =====
            if (lowerMsg.Contains("exit") || lowerMsg.Contains("quit") || lowerMsg.Contains("goodbye"))
            {
                AddBotMessage($"👋 Goodbye {(string.IsNullOrEmpty(userName) ? "" : userName)}! Stay safe online!");
                LogActivity("Session", "User ended session");
                return;
            }

            // ===== HANDLE NAME MEMORY =====
            if ((lowerMsg.Contains("my name is") || lowerMsg.Contains("i'm ") || lowerMsg.Contains("i am ")) && string.IsNullOrEmpty(userName))
            {
                string name = ExtractName(message);
                if (!string.IsNullOrEmpty(name))
                {
                    userName = name;
                    userMemory["user_name"] = userName;
                    statusText.Text = $"● Online - {userName}";
                    AddBotMessage($"👤 Nice to meet you, {userName}! I'll remember your name.");
                    LogActivity("Memory", $"User name stored: {userName}");
                    return;
                }
            }

            if (lowerMsg.Contains("what is my name") && !string.IsNullOrEmpty(userName))
            {
                AddBotMessage($"Your name is {userName}! I remember you! 😊");
                return;
            }

            // ===== HANDLE SHOW MORE LOG =====
            if (lowerMsg.Contains("show more log") || lowerMsg.Contains("full log"))
            {
                AddBotMessage(GetFullActivityLog());
                return;
            }

            // ===== NLP INTENT DETECTION =====
            string intent = GetNLPIntent(message);

            switch (intent)
            {
                case "add_task":
                    HandleAddTask(message);
                    return;

                case "set_reminder":
                    HandleSetReminder(message);
                    return;

                case "show_tasks":
                    AddBotMessage(GetTasksList());
                    LogActivity("Task", "User viewed tasks");
                    return;

                case "complete_task":
                    AddBotMessage(CompleteTask(message));
                    return;

                case "delete_task":
                    AddBotMessage(DeleteTask(message));
                    return;

                case "show_log":
                    AddBotMessage(GetActivityLog());
                    LogActivity("Log", "User viewed activity log");
                    return;

                case "start_quiz":
                    StartQuiz();
                    return;

                case "show_help":
                    ShowHelpMenu();
                    return;
            }

            // ===== SENTIMENT DETECTION =====
            if (lowerMsg.Contains("worried") || lowerMsg.Contains("scared") || lowerMsg.Contains("nervous"))
            {
                AddBotMessage("😟 I understand your concern. Cybersecurity can feel overwhelming, but I'm here to help you step by step!");
                LogActivity("Sentiment", "User expressed worry");
                return;
            }

            if (lowerMsg.Contains("frustrated") || lowerMsg.Contains("confused") || lowerMsg.Contains("annoyed"))
            {
                AddBotMessage("😤 I hear your frustration. Let me simplify this for you!");
                LogActivity("Sentiment", "User expressed frustration");
                return;
            }

            if (lowerMsg.Contains("curious") || lowerMsg.Contains("interested"))
            {
                AddBotMessage("🤓 Great curiosity! That's the first step to better security!");
                LogActivity("Sentiment", "User expressed curiosity");
                return;
            }

            if (lowerMsg.Contains("thank") || lowerMsg.Contains("thanks"))
            {
                AddBotMessage("🙏 You're very welcome! Stay safe online!");
                return;
            }

            // ===== KEYWORD DETECTION =====
            bool found = false;
            foreach (string topic in topics)
            {
                if (lowerMsg.Contains(topic))
                {
                    currentTopic = topic;
                    var tips = responses[topic];
                    string tip = tips[random.Next(tips.Length)];
                    string topicDisplay = topic == "2fa" ? "2FA" : topic.ToUpper();
                    AddBotMessage($"🔒 Here's what I know about {topicDisplay}:\n\n{tip}\n\nWould you like another tip?");
                    found = true;
                    LogActivity("Keyword", $"User asked about {topic}");
                    break;
                }
            }

            // ===== FOLLOW-UP QUESTIONS =====
            if (!found && (lowerMsg.Contains("another tip") || lowerMsg.Contains("more tips") || lowerMsg.Contains("tell me more")))
            {
                if (!string.IsNullOrEmpty(currentTopic) && responses.ContainsKey(currentTopic))
                {
                    var tips = responses[currentTopic];
                    string tip = tips[random.Next(tips.Length)];
                    AddBotMessage($"📚 Another tip about {currentTopic}:\n\n{tip}");
                    LogActivity("Follow-up", $"User asked for more on {currentTopic}");
                    return;
                }
                else
                {
                    AddBotMessage("Please ask about a specific topic first (like passwords, phishing, or privacy) so I can share more tips!");
                    return;
                }
            }

            // ===== DEFAULT RESPONSE =====
            if (!found)
            {
                string[] defaultResponses = {
                    "🤔 I'm not sure I understand. Try asking about passwords, phishing, malware, scams, or privacy.",
                    "💭 Type 'help' to see all the things I can do! I can manage tasks, run quizzes, and more!",
                    "🔄 I can help with tasks, reminders, quizzes, and cybersecurity tips. What would you like?",
                    "📚 Try: 'add task', 'start quiz', or ask me about cybersecurity topics!"
                };
                AddBotMessage(defaultResponses[random.Next(defaultResponses.Length)]);
                LogActivity("Unknown", $"User input: {message}");
            }

            // ===== PERSONALIZED RESPONSE =====
            if (!string.IsNullOrEmpty(userName) && random.Next(8) == 0)
            {
                AddBotMessage($"💙 Keep up the great work, {userName}!");
            }
        }

        // ==================== NLP HANDLERS ====================
        private void HandleAddTask(string input)
        {
            string taskText = ExtractTaskDetails(input);

            if (string.IsNullOrWhiteSpace(taskText))
            {
                AddBotMessage("⚠️ Please specify a task. Example: 'add task - Enable two-factor authentication'");
                return;
            }

            DateTime? reminderDate = ExtractReminderDate(input);

            if (reminderDate.HasValue)
            {
                string description = taskText;
                string datePattern = @"\d{1,2}/\d{1,2}/\d{4}|\d{4}-\d{2}-\d{2}|in \d+ days|tomorrow";
                var match = System.Text.RegularExpressions.Regex.Match(description, datePattern);
                if (match.Success)
                {
                    description = description.Replace(match.Value, "").Trim();
                }

                string result = AddTask(taskText, description, reminderDate);
                AddBotMessage(result);
                LogActivity("Task", $"Added task: {taskText} with reminder on {reminderDate.Value:MMM dd, yyyy}");
            }
            else
            {
                string result = AddTask(taskText, taskText);
                AddBotMessage(result + "\n\n💡 Type 'set reminder' to add a reminder to this task.");
                LogActivity("Task", $"Added task: {taskText}");
            }
        }

        private void HandleSetReminder(string input)
        {
            string taskText = ExtractTaskDetails(input);
            DateTime? reminderDate = ExtractReminderDate(input);

            if (string.IsNullOrWhiteSpace(taskText) || !reminderDate.HasValue)
            {
                AddBotMessage("⚠️ Please specify a task and date. Example: 'remind me to update password in 3 days'");
                return;
            }

            // Check if task exists
            foreach (var task in tasks)
            {
                if (task.Title.ToLower().Contains(taskText.ToLower()))
                {
                    task.ReminderDate = reminderDate;
                    LogActivity("Reminder", $"Updated reminder for task: {task.Title}");
                    AddBotMessage($"✅ Reminder set for '{task.Title}' on {reminderDate.Value:MMM dd, yyyy} at {reminderDate.Value:HH:mm}");
                    return;
                }
            }

            // If task doesn't exist, create it
            string result = AddTask(taskText, taskText, reminderDate);
            AddBotMessage($"✅ New task added with reminder:\n{result}");
            LogActivity("Reminder", $"New task with reminder: {taskText}");
        }

        // ==================== HELP MENU ====================
        private void ShowHelpMenu()
        {
            string help = @"📖 **CYBERSECURITY CHATBOT - HELP MENU**

🔐 **Cybersecurity Topics:**
   Ask about: passwords, phishing, malware, scams, privacy, 2FA, safe browsing

📋 **Task Management:**
   • 'add task' - Add a new task
   • 'show tasks' - View all tasks
   • 'complete task' - Mark task as done
   • 'delete task' - Remove a task
   • 'remind me' - Set a reminder

🎯 **Quiz:**
   • 'start quiz' - Begin cybersecurity quiz
   • 'quit quiz' - Exit quiz

📊 **Activity Log:**
   • 'show log' - View recent activities
   • 'show more log' - View full log

💡 **Other Commands:**
   • 'help' - Show this menu
   • 'my name is' - Tell me your name
   • 'what is my name' - Recall your name
   • 'another tip' - Get more cybersecurity tips

📌 **Examples:**
   • 'add task - Enable two-factor authentication'
   • 'remind me to update password in 3 days'
   • 'show tasks'
   • 'start quiz'";

            AddBotMessage(help);
            LogActivity("Help", "User viewed help menu");
        }

        // ==================== UTILITY METHODS ====================
        private string ExtractName(string input)
        {
            string lower = input.ToLower();
            if (lower.Contains("my name is"))
            {
                int idx = lower.IndexOf("my name is") + 10;
                if (idx < input.Length) return input.Substring(idx).Trim().Split(' ')[0];
            }
            else if (lower.Contains("i'm "))
            {
                int idx = lower.IndexOf("i'm ") + 4;
                if (idx < input.Length) return input.Substring(idx).Trim().Split(' ')[0];
            }
            else if (lower.Contains("i am "))
            {
                int idx = lower.IndexOf("i am ") + 5;
                if (idx < input.Length) return input.Substring(idx).Trim().Split(' ')[0];
            }
            return null;
        }

        private void ShowQuickMenu_Click(object sender, RoutedEventArgs e)
        {
            string menu = "⚡ Quick Topics:\n\n" +
                         "• password\n" +
                         "• phishing\n" +
                         "• malware\n" +
                         "• scam\n" +
                         "• privacy\n" +
                         "• 2fa\n" +
                         "• safe browsing\n\n" +
                         "Type any topic to learn more!";
            AddBotMessage(menu);
        }

        private void ToggleVoice_Click(object sender, RoutedEventArgs e)
        {
            if (synthesizer == null)
            {
                AddBotMessage("⚠️ Voice is not available on this system.");
                return;
            }

            isVoiceEnabled = !isVoiceEnabled;
            btnVoice.Content = isVoiceEnabled ? "🔊" : "🔇";
            AddBotMessage(isVoiceEnabled ? "🔊 Voice responses enabled!" : "🔇 Voice responses disabled!");
            LogActivity("Voice", $"Voice {(isVoiceEnabled ? "enabled" : "disabled")}");
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            chatPanel.Children.Clear();
            AddBotMessage("Chat cleared! How can I help you?");
        }

        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                chatScrollViewer.ScrollToBottom();
            }));
        }

        private void SaveToHistory(string sender, string message)
        {
            try
            {
                string logDir = "ChatHistory";
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                string logFile = Path.Combine(logDir, $"{DateTime.Now:yyyy-MM-dd}.txt");
                string logEntry = $"{DateTime.Now:HH:mm:ss} [{sender}] {message}";
                File.AppendAllText(logFile, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save: {ex.Message}");
            }
        }

        // ==================== SUPPORTING CLASSES ====================
        public class QuizQuestion
        {
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public int CorrectAnswer { get; set; }
            public string Explanation { get; set; }
        }

        public class ActivityLogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
        }

        public class TaskItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderDate { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        private class btnVoice
        {
            internal static string Content;
        }
    }
}