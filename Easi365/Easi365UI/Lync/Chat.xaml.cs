using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Easi365UI.Windows.Controls;
using System.Windows.Input;
using ClientLib.Core;

namespace Easi365UI.Lync
{
    /// <summary>
    /// Chat.xaml 的交互逻辑
    /// </summary>
    public partial class Chat : EasiWindow
    {
        LyncClient _lyncClient;
        Dispatcher _dispatcher;

        public Guid ID;
        public Conversation _Conversation;

        private Contact _Self;

        public ObservableCollection<LyncContract> ContractList { get; set; }

        public Chat(IList<LyncContract> contracts)
        {
            InitializeComponent();
            _dispatcher = Dispatcher.CurrentDispatcher;

            this.ContractList = new ObservableCollection<LyncContract>();
            foreach (var contract in contracts)
                this.ContractList.Add(contract);

            this.DataContext = this;
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _lyncClient = LyncClient.GetClient();
                _Self = _lyncClient.Self.Contact;

                if (_Conversation != null)
                    this.AddEvent(_Conversation);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - Chat_Loaded", lce);
            }
        }

        //发送消息
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMessage.Text)) return;

            if (_Conversation == null ||
                _Conversation.Modalities[ModalityTypes.InstantMessage].State == ModalityState.Disconnected)
            {
                this.AddNewConversation();
            }

            string message = string.Empty;
            _dispatcher.Invoke(new Action(delegate()
            {
                message = txtMessage.Text;
                txtMessage.Text = "";
            }), null);

            InstantMessageModality imModality = (InstantMessageModality)_Conversation.Modalities[ModalityTypes.InstantMessage];

            IDictionary<InstantMessageContentType, string> textMessage = new Dictionary<InstantMessageContentType, string>();
            textMessage.Add(InstantMessageContentType.PlainText, message);

            if (imModality.CanInvoke(ModalityAction.SendInstantMessage))
            {
                Object[] asyncState = { textMessage, _Conversation.Participants, ((InstantMessageModality)_Conversation.Modalities[ModalityTypes.InstantMessage]) };
                IAsyncResult asyncResult = imModality.BeginSendMessage(
                    textMessage,
                    SendMessageCallback,
                    asyncState);

                Log(string.Format("我  {0}", DateTime.Now.ToString("HH:mm:ss")), false, Brushes.Blue, new Thickness(0, 15, 0, 6));
                Log(message, false, Brushes.Black, new Thickness(0, 0, 0, 0));
            }
        }

        //添加新对话
        void AddNewConversation()
        {
            _Conversation = _lyncClient.ConversationManager.AddConversation();
            _Conversation.ParticipantAdded += Conversation_ParticipantAdded;
            _Conversation.ParticipantRemoved += Conversation_ParticipantRemoved;

            foreach (var contract in this.ContractList)
                _Conversation.AddParticipant(contract._Contact);

            ChatForm form = FormManager.GetByID(this.ID);
            if (form != null)
                form.ConversationId = (string)_Conversation.Properties[ConversationProperty.Id];
        }

        //添加联系人后注册接收消息事件
        void Conversation_ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    //如果多于两位参与者，将自己添加到ListBox中
                    if (_Conversation.Participants.Count > 2)
                    {
                        ChatForm form = FormManager.GetByID(this.ID);
                        if (form != null) form.HasMultiParticipants = true;

                        this.AddToContactList(e.Participant.Contact);
                        this.AddToContactList(_lyncClient.Self.Contact);
                    }
                    else
                    {
                        if (e.Participant.Contact.Uri != _lyncClient.Self.Contact.Uri)
                            this.AddToContactList(e.Participant.Contact);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("Lync Exception - System Exception - Conversation_ParticipantAdded", ex);
                }
            }), null);

            if (!e.Participant.IsSelf)
            {
                if (((Conversation)sender).Modalities.ContainsKey(ModalityTypes.InstantMessage))
                {
                    ((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived += ConversationTest_InstantMessageReceived;
                    ((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).IsTypingChanged += ConversationTest_IsTypingChanged;
                }
            }
        }

        //移除联系人
        void Conversation_ParticipantRemoved(object sender, ParticipantCollectionChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    if (!e.Participant.IsSelf)
                    {
                        ((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived -= ConversationTest_InstantMessageReceived;
                        ((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).IsTypingChanged -= ConversationTest_IsTypingChanged;

                        //移除被删的参与者
                        this.RemoveToContactList(e.Participant.Contact.Uri);

                        //如果会话中少于3位参与者，则将自己在ListBox中移除
                        if (_Conversation.Participants.Count <= 2)
                        {
                            if (_Conversation.Participants.Count == 1 &&
                                _Conversation.Participants[0].Contact.Uri == _lyncClient.Self.Contact.Uri)
                                this.AddToContactList(_Self);
                            else
                                this.RemoveToContactList(_Self.Uri);
                        }
                    }
                    else  //如果删除的是自己，则ListBox中只显示自己
                    {
                        this.ContractList.Clear();
                        this.AddToContactList(_Self);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("Lync Exception - System Exception - Conversation_ParticipantRemoved", ex);
                }
            }), null);
        }

        //添加至ListBox
        void AddToContactList(Contact contact)
        {
            LyncContract contract = new LyncContract(contact, _dispatcher);

            if (!this.ContractList.Contains<LyncContract>(contract, LyncContractComparer.Default))
                this.ContractList.Add(contract);
        }
        void RemoveToContactList(string uri)
        {
            var contract = this.ContractList.Where(m => m.Uri == uri).FirstOrDefault();
            if (contract != null)
                this.ContractList.Remove(contract);
        }

        //注册接收消息事件
        public void AddEvent(Conversation conversation)
        {
            if (conversation != null && conversation.Modalities.ContainsKey(ModalityTypes.InstantMessage))
            {
                conversation.ParticipantAdded += Conversation_ParticipantAdded;
                conversation.ParticipantRemoved += Conversation_ParticipantRemoved;
            }
        }

        private void SendMessageCallback(IAsyncResult ar)
        {
            if (ar.IsCompleted == true)
            {
                Object[] _asyncState = (Object[])ar.AsyncState;
                IDictionary<InstantMessageContentType, string> textMessage = (IDictionary<InstantMessageContentType, string>)_asyncState[0];
                IList<Participant> participants = (IList<Participant>)_asyncState[1];
                try
                {
                    ((InstantMessageModality)_asyncState[2]).EndSendMessage(ar);
                }
                catch (LyncClientException ex)
                {
                    FailedSendMessage(ex.InternalCode, textMessage);
                }
            }
        }

        //发送失败的消息处理
        private void FailedSendMessage(uint exceptionCode, IDictionary<InstantMessageContentType, string> textMessage)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    string displayName = string.Empty;
                    foreach (var contract in this.ContractList)
                    {
                        if (contract.Uri == _Self.Uri) continue;
                        displayName = contract.DisplayName;
                        break;
                    }

                    string imagePath = @"pack://application:,,,/Assets/Images/error.png";
                    if (exceptionCode == 2163395378)
                    {
                        Log(string.Format(" 未发送此消息，因为 {0} 不想被打扰。", string.IsNullOrEmpty(displayName) ? "该用户" : displayName), false, Brushes.Gray, new Thickness(0, 15, 0, 6), imagePath);
                    }
                    else if (exceptionCode == 2163147232)
                    {
                        Log(string.Format(" 无法发送此消息，因为 {0} 没空或脱机。", string.IsNullOrEmpty(displayName) ? "该用户" : displayName), false, Brushes.Gray, new Thickness(0, 15, 0, 6), imagePath);
                    }
                    else
                    {
                        Log(" 可能由于网络原因，消息发送失败", false, Brushes.Gray, new Thickness(0, 15, 0, 6), imagePath);
                    }
                    Log(textMessage[InstantMessageContentType.PlainText], false, Brushes.Black, new Thickness(0, 0, 0, 0));
                }
                catch (Exception ex)
                {
                }
            }), null);
        }

        void ConversationTest_IsTypingChanged(object sender, IsTypingChangedEventArgs e)
        {

        }

        //接收到消息处理
        void ConversationTest_InstantMessageReceived(object sender, MessageSentEventArgs e)
        {
            string message = string.Empty;
            string displayName = ((InstantMessageModality)sender).Participant.Contact.GetContactInformation(ContactInformationType.DisplayName).ToString();
            Log(string.Format("{0}  {1}", displayName, DateTime.Now.ToString("HH:mm:ss")), false, Brushes.ForestGreen, new Thickness(0, 15, 0, 6));
            if (e.Contents.TryGetValue(InstantMessageContentType.RichText, out message))
            {
                Log(message, true, Brushes.Black, new Thickness(0, 0, 0, 0));
                return;
            }
            if (e.Contents.TryGetValue(InstantMessageContentType.PlainText, out message))
            {
                Log(message, false, Brushes.Black, new Thickness(0, 0, 0, 0));
                return;
            }
            if (e.Contents.TryGetValue(InstantMessageContentType.Html, out message))
            {
                Log(Common.FilertHTML(message), false, Brushes.Black, new Thickness(0, 0, 0, 0));
                return;
            }
        }

        //收到消息填充到RichTextBox
        private int Sign = 0;
        private void Log(string message, bool isRichText, SolidColorBrush brush, Thickness margin)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                FlowDocument doc = txtLog.Document;

                Paragraph para = new Paragraph();
                para.Margin = margin;
                para.LineHeight = 8;
                if (isRichText)
                {
                    TextRange range = new TextRange(para.ContentStart, para.ContentEnd);
                    byte[] byteArray = Encoding.ASCII.GetBytes(message);
                    MemoryStream stream = new MemoryStream(byteArray);
                    range.Load(stream, DataFormats.Rtf);
                    para.Inlines.Add(range.Text.Replace("\r\n", ""));
                }
                else
                {
                    para.Inlines.Add(new Run(message) { Foreground = brush });
                }

                doc.Blocks.Add(para);
                if (Sign == 0)
                {
                    Sign++;
                    doc.Blocks.Remove(para.PreviousBlock);
                }
                this.txtLog.Document = doc;
                this.txtLog.ScrollToEnd();
            }), null);
        }
        private void Log(string message, bool isRichText, SolidColorBrush brush, Thickness margin, string imagePath)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                FlowDocument doc = txtLog.Document;

                Paragraph para = new Paragraph();
                para.Margin = margin;
                para.LineHeight = 8;
                if (isRichText)
                {
                    TextRange range = new TextRange(para.ContentStart, para.ContentEnd);
                    byte[] byteArray = Encoding.ASCII.GetBytes(message);
                    MemoryStream stream = new MemoryStream(byteArray);
                    range.Load(stream, DataFormats.Rtf);
                    para.Inlines.Add(range.Text.Replace("\r\n", ""));
                }
                else
                {

                    BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = bitmap.Width;
                    image.Height = bitmap.Height;
                    para.Inlines.Add(image);
                    para.Inlines.Add(new Run(message) { Foreground = brush, BaselineAlignment = BaselineAlignment.Center });
                }

                doc.Blocks.Add(para);
                this.txtLog.Document = doc;
                this.txtLog.ScrollToEnd();
            }), null);
        }

        #region 窗口操作

        // 最小化窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        // 最大化窗体
        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            ToMaxWindow();
        }

        //双击头部最大化窗体
        private void ContentHeader_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToMaxWindow();
        }

        private void ToMaxWindow()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.MaxBtn.ToolTip = "还原";
                this.WindowState = WindowState.Normal;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/max.png"));
                this.MaxBtn.Background = brush;
                brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/maxb.png"));
                this.MaxBtn.MyEnterBrush = brush;
                this.MaxBtn.MyMoverBrush = brush;
            }
            else
            {
                this.MaxBtn.ToolTip = "最大化";
                this.WindowState = WindowState.Maximized;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/maxa.png"));
                this.MaxBtn.Background = brush;
                brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/maxab.png"));
                this.MaxBtn.MyEnterBrush = brush;
                this.MaxBtn.MyMoverBrush = brush;
            }
        }

        #endregion

        #region 退出聊天窗口

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Chat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    if (_Conversation != null)
                    {
                        _Conversation.End();
                    }
                }
                catch (Exception ex)
                {
                }
                FormManager.Remove(this.ID);
            }
        }

        #endregion

        #region 邀请用户

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Invite inviteForm = new Invite();
            inviteForm.ShowDialog();
            if (!string.IsNullOrEmpty(inviteForm.Uri))
            {
                Contact contact = _lyncClient.ContactManager.GetContactByUri(inviteForm.Uri);
                if (contact != null)
                {
                    foreach (Participant participant in _Conversation.Participants)
                    {
                        if (participant.Contact.Uri == contact.Uri) return;
                    }
                    _Conversation.AddParticipant(contact);
                }
            }
        }

        #endregion
    }
}
