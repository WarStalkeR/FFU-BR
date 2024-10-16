using System;

namespace FFU_Beyond_Reach {
    public static class ModLog {
        public static void Init() {
            UnityEngine.Debug.Log($"Initialized: {FFU_BR_Defs.ModName} v{FFU_BR_Defs.ModVersion}, {DateTime.Now}");
        }
        public static void Info(string logEntry = "") {
            UnityEngine.Debug.Log($"{logEntry}");
        }
        public static void Debug(string logEntry = "") {
            UnityEngine.Debug.Log($"{logEntry}");
        }
        public static void Message(string logEntry = "") {
            UnityEngine.Debug.LogWarning($"{logEntry}");
        }
        public static void Warning(string logEntry = "") {
            UnityEngine.Debug.LogWarning($"{logEntry}");
        }
        public static void Error(string logEntry = "") {
            UnityEngine.Debug.LogError($"{logEntry}");
        }
        public static void Fatal(string logEntry = "") {
            UnityEngine.Debug.LogError($"{logEntry}");
        }
    }
}
