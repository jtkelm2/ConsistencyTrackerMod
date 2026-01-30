using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Mod.ConsistencyTracker.Stats {
    public static class AttemptLogger {
        
        private static string LogFilePath => ConsistencyTrackerModule.GetPathToFile(ConsistencyTrackerModule.StatsFolder, "attempt-history.json");
        
        private static Dictionary<string, Dictionary<string, List<bool>>> _data = null;
        
        private static void EnsureLoaded() {
            if (_data != null) return;
            
            if (File.Exists(LogFilePath)) {
                try {
                    string json = File.ReadAllText(LogFilePath);
                    _data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<bool>>>>(json);
                } catch {
                    _data = new Dictionary<string, Dictionary<string, List<bool>>>();
                }
            } else {
                _data = new Dictionary<string, Dictionary<string, List<bool>>>();
            }
        }
        
        public static void LogAttempt(string chapterDebugName, string roomDebugName, bool success) {
            EnsureLoaded();
            
            if (!_data.ContainsKey(chapterDebugName)) {
                _data[chapterDebugName] = new Dictionary<string, List<bool>>();
            }
            
            if (!_data[chapterDebugName].ContainsKey(roomDebugName)) {
                _data[chapterDebugName][roomDebugName] = new List<bool>();
            }
            
            _data[chapterDebugName][roomDebugName].Add(success);
            
            Save();
        }
        
        private static void Save() {
            try {
                string json = JsonConvert.SerializeObject(_data, Formatting.Indented);
                File.WriteAllText(LogFilePath, json);
            } catch (Exception ex) {
                ConsistencyTrackerModule.Instance.Log($"Failed to save attempt history: {ex}");
            }
        }
    }
}