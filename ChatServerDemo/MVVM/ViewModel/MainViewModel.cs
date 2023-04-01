using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        private Server _server;
        public RelayCommand ConnectToServerCommand{ get; set; }
        public RelayCommand SendNewMessageCommand { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }

        public MainViewModel()  
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.disconnectEvent += UserDisconnected;
            _server.messageReceivedEvent += MessageReceived;

            ConnectToServerCommand = new RelayCommand(
                o => _server.ConnectToServer(Username), 
                o => !string.IsNullOrEmpty(Username)
            );
            SendNewMessageCommand = new RelayCommand(
                o => _server.SendMessageToServer(Message),
                o => !string.IsNullOrEmpty(Message));
        }

        private void UserDisconnected()
        {
            var uid = _server.packetRader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Remove(user);
            });
        }

        private void MessageReceived()
        {
            var message = _server.packetRader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(message);
            });
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
