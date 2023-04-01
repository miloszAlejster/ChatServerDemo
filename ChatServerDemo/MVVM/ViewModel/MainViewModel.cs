using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        private Server _server;
        public RelayCommand ConnectToServerCommand{ get; set; }
        public string Username { get; set; }

        public MainViewModel()  
        {
            Users = new ObservableCollection<UserModel>();
            _server = new Server();
            ConnectToServerCommand = new RelayCommand(
                o => _server.ConnectToServer(Username), 
                o => !string.IsNullOrEmpty(Username)
            );
            _server.connectedEvent += UserConnected;

        }
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.packetRader.ReadMessage(),
                UID = _server.packetRader.ReadMessage()
            };

            if(!Users.Any(x => x.UID == user.UID)) 
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Add(user);
                });
            }
        }
    }
}
