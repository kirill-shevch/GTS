using GtsServer.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoneLabWebApp.Services.Hubs
{
    public class UserHub : Hub
    {
        private Dictionary<string, Player> _players = new Dictionary<string, Player>();

        public UserHub()
        {
        }

        public async Task AddUserName(string userName)
        {
            _players.Add(userName, new Player
            {
                Name = userName
            });
            await SendUser(_players[userName]);
        }

        public async Task RemoveUserName(string userName)
        {
            _players.Remove(userName);
            await RemoveUser(userName);
        }

        public async Task Synchronize(Player player)
        {
            if (_players.ContainsKey(player.Name))
            {
                _players[player.Name] = player;
                await SendUser(player);
            }
        }

        public async Task SendUser(Player player)
        {
            await Clients?.All.SendAsync("SendUser", player);
        }

        public async Task RemoveUser(string playerName)
        {
            await Clients?.All.SendAsync("RemoveUser", playerName);
        }
    }
}
