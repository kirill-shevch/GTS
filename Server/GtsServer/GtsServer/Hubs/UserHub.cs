using GtsServer.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoneLabWebApp.Services.Hubs
{
    public class UserHub : Hub
    {
        private Dictionary<string, ServerPlayer> _players = new Dictionary<string, ServerPlayer>();

        public UserHub()
        {
        }

        public async Task AddUserName(string userName, string connectionId)
        {
            _players.Add(userName, new ServerPlayer
            {
                Name = userName,
                ConnectionId = connectionId
            });
            await SendUser(_players[userName]);
        }

        public async Task RemoveUserName(string userName)
        {
            _players.Remove(userName);
            await RemoveUser(userName);
        }

        public async Task Synchronize(ServerPlayer player)
        {
            if (_players.ContainsKey(player.Name))
            {
                _players[player.Name] = player;
                await SendUser(player);
            }
        }

        public async Task SendUser(ServerPlayer player)
        {
            await Clients?.AllExcept(new List<string> { player.ConnectionId }).SendAsync("SendUser", player);
        }

        public async Task RemoveUser(string playerName)
        {
            await Clients?.All.SendAsync("RemoveUser", playerName);
        }

        public async Task CreateProjectile(float x, float z, Direction direction)
        {
            await Clients?.All.SendAsync("Shoot", x, z, direction);
        }
    }
}
