using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int localPort = 8080;
        public Task task;
        public CancellationTokenSource cancelTokenSource;
        public CancellationToken token;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// По клику на Старт запускаем поток, принимающий данные по UDP
        /// </summary>
        private void ButtonClickStart(object sender, RoutedEventArgs e)
        {
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            task = new Task(
                () => ReceiveMessage(token)
            );
            task.Start();
        }

        /// <summary>
        /// Завершаем поток по клику на Стоп
        /// </summary>
        private void ButtonClickStop(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();
            ChangeTextBlock("0");
        }

        /// <summary>
        /// Прослушка порта
        /// </summary>
        private void ReceiveMessage(CancellationToken token)
        {
            UdpClient receiver = new UdpClient(localPort);
            IPEndPoint remoteIp = null;
            try
            {
                while (true)
                {
                    byte[] data = receiver.Receive(ref remoteIp);
                    string message = Encoding.Unicode.GetString(data);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        ChangeTextBlock(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }

        /// <summary>
        /// Изменение значения индикатора
        /// </summary>
        private void ChangeTextBlock(string message)
        {
            textBlock.Dispatcher.BeginInvoke(
                new Action(
                    delegate ()
                    {
                        textBlock.Text = message;

                    }
                )
            );
        }
    }
}
