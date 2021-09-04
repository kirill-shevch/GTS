﻿using GtsServer.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoneLabWebApp.Services.Hubs
{
    public class UserHub : Hub
    {
        private Dictionary<string, ServerPlayer> _players = new Dictionary<string, ServerPlayer>();
        private Dictionary<string, KillDeathAmount> KDTable = new Dictionary<string, KillDeathAmount>();

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
            KDTable.Add(userName, new KillDeathAmount());
            await SendUser(_players[userName]);
        }

        public async Task IncrementKill(string userName)
        {
            KDTable[userName].Kills++;
            await SendMoney(userName);
            await BroadcastKDTable();
        }        
        
        public async Task IncrementDeath(string userName)
        {
            KDTable[userName].Deathes++;
            await BroadcastKDTable();
        }

        public async Task BroadcastKDTable()
        {
            await Clients?.All.SendAsync("BroadcastKDTable", KDTable);
        }

        public async Task SendMoney(string userName)
        {
            await Clients?.Client(_players[userName].ConnectionId).SendAsync("GetMoney");
        }

        public async Task RemoveUserName(string userName)
        {
            _players.Remove(userName);
            KDTable.Remove(userName);
            await RemoveUser(userName);
            await Clients?.All.SendAsync("BroadcastKDTable", KDTable);
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

        public async Task CreateProjectile(float x, float z, Direction direction, string shooterName)
        {
            await Clients?.All.SendAsync("Shoot", x, z, direction, shooterName);
        }
    }
}
