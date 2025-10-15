// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using PixelCrushers.DialogueSystem;

// public class DialogueTimerUI : MonoBehaviour
// {
//     public static DialogueTimerUI Instance;
//     public TextMeshProUGUI timerText;
//     private float timer;
//     private bool isCounting = false;
//     private float averageDurationPerLine = 3f;

//     void Awake()
//     {
//         Instance = this;
//         timerText.gameObject.SetActive(false);
//     }

//     void Update()
//     {
//         if (isCounting)
//         {
//             timer -= Time.deltaTime;
//             timerText.color = timer <= 5f ? Color.red : Color.white;

//             if (timer <= 0f)
//             {
//                 timer = 0f;
//                 isCounting = false;
//                 timerText.gameObject.SetActive(false);
//                 DialogueManager.StopConversation();
//             }

//             timerText.text = $"{Mathf.CeilToInt(timer)}";
//         }
//     }

//     // Call this right after StartConversation()
//     public void StartTimerForConversation(string conversationName)
//     {
//         var conversation = DialogueManager.MasterDatabase.GetConversation(conversationName);
//         if (conversation != null)
//         {
//             int lineCount = conversation.dialogueEntries != null ? conversation.dialogueEntries.Count : 0;
//             float duration = lineCount * averageDurationPerLine;
//             StartTimer(duration);
//         }
//         else
//         {
//             // fallback if not found
//             StartTimer(10f);
//         }
//     }

//     public void StartTimer(float duration)
//     {
//         timer = duration;
//         isCounting = true;
//         timerText.gameObject.SetActive(true);
//     }

//     public void StopTimer()
//     {
//         isCounting = false;
//         timerText.gameObject.SetActive(false);
//     }

//     // This method listens for Dialogue System sequencer broadcasts.
//     public void OnSequencerMessage(string message)
//     {
//         if (message.StartsWith("MarkNPC:"))
//         {
//             string npcName = message.Substring("MarkNPC:".Length);
//             Debug.Log($"Received MarkNPC message for {npcName}");
//             // send message to Dialogue System
//             DialogueManager.Instance.SendMessage("OnSequencerMessage", message);
//         }
//     }
// }