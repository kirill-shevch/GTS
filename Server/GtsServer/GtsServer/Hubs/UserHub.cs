using GtsServer.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Name = userName,
                Color = GetRandomColor()
            });
            await SendUserList(_players);
        }

        public async Task RemoveUserName(string userName)
        {
            _players.Remove(userName);
            await SendUserList(_players);
        }

        public async Task SetCoordinate(string userName, int x, int y)
        {
            _players[userName].X = x;
            _players[userName].Y = y;
            await SendUserList(_players);
        }

        public async Task SendUserList(Dictionary<string, Player> userNames)
        {
            await Clients?.All.SendAsync("ReceiveUserList", userNames);
        }

        private string GetRandomColor()
        {
            var rand = new Random();
            var letters = "0123456789ABCDEF".ToCharArray();
            var color = new StringBuilder("#");
            for (var i = 0; i < 6; i++)
            {
                color.Append(letters[rand.Next(0, 15)]);
            }
            return color.ToString();
        }
    }
}
