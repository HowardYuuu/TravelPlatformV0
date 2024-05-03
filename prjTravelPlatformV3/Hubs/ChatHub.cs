using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using prjTravelPlatformV3.Models;
using System.Security.Claims;

namespace CoreMVC_SignalR_Chat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;
        public ChatHub(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public class ConnectionInfo
        {
            public string? ConnectString { get; set; }
            public string? MemberId { get; set; }
            public string? MemberName { get; set; }
        }

        public class MessageInfo
        {
            public string? SelfId { get; set; }
            public string? SendToId { get; set; }
            public string? Message { get; set; }
            public string? SelfMemberId { get; set; }
            public string? SendToMemberId { get; set; }
            public string? TimeStamp { get; set; }
        }

        //員工數量
        public static int EmployeeCount = 0;

        //聊天歷史紀錄
        public static List<TMessageInfo> MessageHistory = new List<TMessageInfo>();

        // 用戶連線 ID 列表
        public static List<ConnectionInfo> ConnList = new List<ConnectionInfo>();

        public async Task ClientConnection(string Character)
        {
            if(Character != "Customer")
            {
                var EmployeeId = "E" + _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var EmployeeName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                //var EmployeeId = "E1";
                //var EmployeeName = "哈哈";


                // 更新個人connection ID
                await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.ConnectionId);

                // 更新個人member ID
                await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfMemberID", EmployeeId);

                // 更新個人MemberName
                await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfName", EmployeeName);

                // 加入員工group
                await Groups.AddToGroupAsync(Context.ConnectionId, "Employees");

                // 更新連線 ID 列表
                string CustomerList = JsonConvert.SerializeObject(ConnList);
                await Clients.Client(Context.ConnectionId).SendAsync("UpdList", CustomerList);

                EmployeeCount++;
                await Clients.All.SendAsync("EmployeeCount", EmployeeCount);
                return;
            }
            var CustomerId = "C" + _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CustomerName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            // 查看ConnList中是否已存在相同的 ConnectionInfo
            if (ConnList.FirstOrDefault(p => p.MemberId == CustomerId) == null)
            {
                ConnectionInfo newConnection = new ConnectionInfo
                {
                    ConnectString = Context.ConnectionId,
                    MemberId = CustomerId,
                    MemberName = CustomerName
                };
                ConnList.Insert(0, newConnection);
            }

            // 更新個人connection ID
            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.ConnectionId);

            // 更新個人member ID
            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfMemberID", CustomerId);

            // 更新個人MemberName
            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfName", CustomerName);

            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnList);
            await Clients.All.SendAsync("UpdList", jsonString);

            //獲取目前員工數量資訊
            await Clients.Client(Context.ConnectionId).SendAsync("EmployeeCount", EmployeeCount);
        }


        //離線
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var disconnectedConnection = ConnList.FirstOrDefault(p => p.ConnectString == Context.ConnectionId);
            if (disconnectedConnection != null)
            {
                ConnList.Remove(disconnectedConnection);
            }
            else //如果離線的是員工
            {
                // 離開員工group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Employees");
                EmployeeCount--;
                await Clients.All.SendAsync("EmployeeCount", EmployeeCount);
            }

            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnList);
            await Clients.All.SendAsync("UpdList", jsonString);

            await base.OnDisconnectedAsync(ex);
        }

        //傳訊息
        public async Task SendMessage(string? selfID, string? message, string? sendToID, string? SelfMemberID, string? sendToMemberID)
        {
            TMessageInfo messageInfo = new TMessageInfo
            {
                Message = message,
                SelfId = selfID,
                SendToId = sendToID,
                SelfMemberId = SelfMemberID,
                SendToMemberId = sendToMemberID,
                TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
            };
            if (string.IsNullOrEmpty(selfID)) { return; }
            if (string.IsNullOrEmpty(sendToID)) { return; }

            string jsonString = JsonConvert.SerializeObject(messageInfo);
            if (SelfMemberID.Contains("E")) // 發送者是客服的話訊息要廣播給所有客服
            {
                await Clients.Group("Employees").SendAsync("UpdContent", jsonString);
            }
            else
            {
                await Clients.Client(selfID).SendAsync("UpdContent", jsonString);
            }
            await Clients.Client(sendToID).SendAsync("UpdContent", jsonString);

            //存到全域的歷史紀錄
            MessageHistory.Add(messageInfo);

            if (MessageHistory.Count() >= 10)
            {
                _context.TMessageInfos.AddRange(MessageHistory);
                _context.SaveChanges();
                MessageHistory.Clear();
            }
        }

        public async Task SendMessageToEmployee(string? selfID, string? message, string? SelfMemberID)
        {
            TMessageInfo messageInfo = new TMessageInfo
            {
                Message = message,
                SelfId = selfID,
                SendToId = "E",
                SelfMemberId = SelfMemberID,
                SendToMemberId = "E",
                TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
            };
            if (string.IsNullOrEmpty(selfID)) { return; }

            string jsonString = JsonConvert.SerializeObject(messageInfo);
            await Clients.Client(selfID).SendAsync("UpdContent", jsonString);
            await Clients.Group("Employees").SendAsync("UpdContent", jsonString);

            //存到全域的歷史紀錄
            MessageHistory.Add(messageInfo);

            if (MessageHistory.Count() >= 10)
            {
                _context.TMessageInfos.AddRange(MessageHistory);
                _context.SaveChanges();
                MessageHistory.Clear();
            }
        }



        //聊天歷史紀錄customer to customer
        public async Task SearchHistory(string? MemberID, string? SelfMemberID, string? selfID)
        {
            if (string.IsNullOrEmpty(MemberID)) { return; }
            if (string.IsNullOrEmpty(SelfMemberID)) { return; }
            if (string.IsNullOrEmpty(selfID)) { return; }


            //撈出DB聊天紀錄中MemberID有對到的
            List<TMessageInfo> MessageHistoryFromDB = _context.TMessageInfos.Where(t => (t.SelfMemberId == MemberID && t.SendToMemberId == SelfMemberID) || (t.SendToMemberId == MemberID && t.SelfMemberId == SelfMemberID)).ToList();
            string jsonDBHistory = JsonConvert.SerializeObject(MessageHistoryFromDB);
            await Clients.Client(selfID).SendAsync("LoadMessageHistoryFromDB", jsonDBHistory);


            //撈出本地聊天紀錄中MemberID有對到的
            List<TMessageInfo> MessageHistoryFromLocal = MessageHistory.Where(t => (t.SelfMemberId == MemberID && t.SendToMemberId == SelfMemberID) || (t.SendToMemberId == MemberID && t.SelfMemberId == SelfMemberID)).ToList();
            string jsonLocalHistory = JsonConvert.SerializeObject(MessageHistoryFromLocal);
            await Clients.Client(selfID).SendAsync("LoadMessageHistoryFromLocal", jsonLocalHistory);

            
        }


        //聊天歷史紀錄customer to employee
        public async Task SearchEmployeeHistory(string? MemberID, string? selfID)
        {
            if (string.IsNullOrEmpty(MemberID)) { return; }


            //撈出DB聊天紀錄中MemberID有對到的
            List<TMessageInfo> MessageHistoryFromDB = _context.TMessageInfos.Where(t => (t.SelfMemberId == MemberID && t.SendToMemberId.Contains("E")) || (t.SendToMemberId == MemberID && t.SelfMemberId.Contains("E"))).ToList();
            string jsonDBHistory = JsonConvert.SerializeObject(MessageHistoryFromDB);
            await Clients.Client(selfID).SendAsync("LoadMessageHistoryFromDB", jsonDBHistory);


            //撈出本地聊天紀錄中MemberID有對到的
            List<TMessageInfo> MessageHistoryFromLocal = MessageHistory.Where(t => (t.SelfMemberId == MemberID && t.SendToMemberId.Contains("E")) || (t.SendToMemberId == MemberID && t.SelfMemberId.Contains("E"))).ToList();
            string jsonLocalHistory = JsonConvert.SerializeObject(MessageHistoryFromLocal);
            await Clients.Client(selfID).SendAsync("LoadMessageHistoryFromLocal", jsonLocalHistory);


        }
    }
}