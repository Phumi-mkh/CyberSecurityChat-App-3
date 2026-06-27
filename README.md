# 🔒 Cybersecurity Awareness Chatbot - Part 3

<div align="center">

![Version](https://img.shields.io/badge/version-2.0-blue)
![C#](https://img.shields.io/badge/C%23-.NET%208.0-purple)
![WPF](https://img.shields.io/badge/UI-WPF-green)
![Database](https://img.shields.io/badge/Database-SQLite-orange)

**A complete cybersecurity chatbot with NLP simulation, task management, quiz, and activity log**

</div>

---

## 📋 Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Requirements Met](#requirements-met)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Database Setup](#database-setup)
- [Usage Guide](#usage-guide)
- [Commands](#commands)
- [Example Interactions](#example-interactions)
- [GitHub Commits](#github-commits)
- [Video Presentation](#video-presentation)
- [Author](#author)

---

## 🎯 Overview

The **Cybersecurity Awareness Chatbot** is a complete WPF application designed to educate users about online safety. It features a WhatsApp-style interface, voice output, memory capabilities, sentiment detection, NLP simulation, task management, a cybersecurity quiz, and an activity log.

### What This Project Does
- **Educates** users about cybersecurity topics (passwords, phishing, malware, scams, privacy, 2FA, safe browsing)
- **Manages** cybersecurity tasks with reminders
- **Tests** knowledge through a 12-question quiz
- **Tracks** all activities in a log
- **Remembers** user name and preferences
- **Responds** to emotions (worried, frustrated, curious)
- **Speaks** responses using text-to-speech

---

## ✨ Features

### Part 1 & 2 Features
| Feature | Description |
|---------|-------------|
| 🎨 **WhatsApp-style UI** | Green header, chat bubbles, typing indicator, timestamps |
| 🔐 **ASCII Art Header** | Box-style "CYBER SECURITY BOT" logo |
| 🗣️ **Voice Output** | Text-to-speech with toggle button |
| 🧠 **Memory Feature** | Remembers user name and recalls it on demand |
| 💬 **Keyword Recognition** | Detects 7 cybersecurity topics |
| 🔄 **Random Responses** | Multiple responses per topic stored in Lists |
| 😊 **Sentiment Detection** | Responds to worried, frustrated, curious, thankful emotions |
| 🔁 **Follow-up Questions** | Handles "another tip", "tell me more" |

### Part 3 Features
| Feature | Description |
|---------|-------------|
| 🤖 **NLP Simulation** | Intent recognition for tasks, reminders, quiz, and log |
| 📋 **Task Management** | Add, view, complete, and delete tasks |
| 💾 **Database Storage** | All tasks and activity logs saved to SQLite database |
| 🎯 **Cybersecurity Quiz** | 12 questions with immediate feedback and scoring |
| 📊 **Activity Log** | Tracks all significant actions with timestamps |
| ⚡ **Quick Action Buttons** | One-click access to Quiz, Tasks, Log, Help |
| 🛡️ **Error Handling** | Graceful handling of unknown inputs |

---

## ✅ Requirements Met

### From Assignment
- ✅ GUI interface with WhatsApp-style design
- ✅ ASCII art integration from Part 1
- ✅ Voice implementation with toggle
- ✅ Keyword recognition (7 cybersecurity topics)
- ✅ Random responses using Lists/Arrays
- ✅ Conversation flow with follow-up questions
- ✅ Memory feature to recall user information
- ✅ Sentiment detection (worried, frustrated, curious)
- ✅ Error handling for unknown inputs
- ✅ Code optimization with OOP practices
- ✅ NLP simulation with intent recognition
- ✅ Task management with reminders
- ✅ Cybersecurity quiz (12 questions)
- ✅ Activity log tracking
- ✅ Database integration (SQLite)

---

## 🛠️ Technologies Used

| Technology | Purpose |
|------------|---------|
| **C# / .NET 8.0** | Core programming language |
| **WPF (Windows Presentation Foundation)** | GUI framework |
| **System.Speech** | Text-to-speech voice output |
| **Entity Framework Core SQLite** | Database ORM |
| **SQLite** | Local database for task storage |
| **Custom NLP** | Intent recognition with keyword patterns |

---

## 🚀 Installation

### Prerequisites
- Windows OS (for voice features)
- .NET 8.0 SDK or later
- Visual Studio 2022 or later (recommended)
- SQLite (included with NuGet package)

### Step-by-Step Installation

1. **Clone the repository**
```bash
git clone https://github.com/YOUR-USERNAME/Cybersecurity-Chatbot-Part3.git
