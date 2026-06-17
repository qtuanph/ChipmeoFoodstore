using Microsoft.AspNetCore.SignalR;

namespace FoodstoreApi.Web.Hubs;

public class AppHub : Hub
{
    // Clients can join specific groups if needed, but for now we broadcast to all
    // or specific groups like "Admin", "POS"
    
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}




