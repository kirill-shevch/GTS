﻿using GtsServer.Models;
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
            await SendUserList(_players);
        }

        public async Task RemoveUserName(string userName)
        {
            _players.Remove(userName);
            await RemoveUser(userName);
        }

        public async Task SetCoordinate(string userName, int x, int z)
        {
            _players[userName].X = x;
            _players[userName].Z = z;
            await SendUserList(_players);
        }

        public async Task SendUserList(Dictionary<string, Player> players)
        {
            await Clients?.All.SendAsync("ReceiveUserList", players);
        }

        public async Task RemoveUser(string playerName)
        {
            await Clients?.All.SendAsync("RemoveUser", playerName);
        }
    }
}
