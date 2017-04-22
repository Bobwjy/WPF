using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace Easi365UI.Lync
{
    public class LyncConversation : INotifyPropertyChanged
    {
        Dispatcher _dispatcher;
        LyncClient _lyncClient;

        public Guid _ID;
        public Conversation _Conversation;
        public LyncConversation(Guid id, Conversation coversation)
        {
            _ID = id;
            _Conversation = coversation;

            _Self = _lyncClient.Self.Contact;

            this.ContractList = new ObservableCollection<LyncContract>();
        }

        Contact _Self;

        private ObservableCollection<LyncContract> contractList;
        public ObservableCollection<LyncContract> ContractList
        {
            get { return contractList; }
            set { contractList = value; OnPropertyChanged("ContractList"); }
        }

        public Conversation InitConversation()
        {
            if (_Conversation != null &&
                _Conversation.Modalities.ContainsKey(ModalityTypes.InstantMessage) &&
                _Conversation.Modalities[ModalityTypes.InstantMessage].State != ModalityState.Disconnected)
            {
                _Conversation.ParticipantAdded += Conversation_ParticipantAdded;
                _Conversation.ParticipantRemoved += Conversation_ParticipantRemoved;
            }
            else
            {
                _Conversation = _lyncClient.ConversationManager.AddConversation();
                _Conversation.ParticipantAdded += Conversation_ParticipantAdded;
                _Conversation.ParticipantRemoved += Conversation_ParticipantRemoved;

                foreach (var contract in this.ContractList)
                    _Conversation.AddParticipant(contract._Contact);

                ChatForm form = FormManager.GetByID(_ID);
                if (form != null)
                    form.ConversationId = (string)_Conversation.Properties[ConversationProperty.Id];
            }

            return _Conversation;
        }

        void Conversation_ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    //如果多于两位参与者，将自己添加到ListBox中
                    if (_Conversation.Participants.Count > 2)
                    {
                        ChatForm form = FormManager.GetByID(_ID);
                        if (form != null) form.HasMultiParticipants = true;

                        this.AddToContactList(e.Participant.Contact);
                        this.AddToContactList(_Self);
                    }
                    else
                    {
                        if (e.Participant.Contact.Uri !=  _Self.Uri)
                            this.AddToContactList(e.Participant.Contact);
                    }
                }
                catch (Exception ex)
                {
                }
            }), null);

            if (!e.Participant.IsSelf)
            {
                if (((Conversation)sender).Modalities.ContainsKey(ModalityTypes.InstantMessage))
                {
                    //((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived += ConversationTest_InstantMessageReceived;
                    //((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).IsTypingChanged += ConversationTest_IsTypingChanged;
                }
            }
        }

        void Conversation_ParticipantRemoved(object sender, ParticipantCollectionChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    if (!e.Participant.IsSelf)
                    {
                        //((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived -= ConversationTest_InstantMessageReceived;
                        //((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).IsTypingChanged -= ConversationTest_IsTypingChanged;

                        //移除被删的参与者
                        this.ContractList.Remove(new LyncContract(e.Participant.Contact, _dispatcher));

                        //如果会话中少于3位参与者，则将自己在ListBox中移除
                        if (_Conversation.Participants.Count <= 2)
                        {
                            if (_Conversation.Participants.Count == 1 &&
                                _Conversation.Participants[0].Contact.Uri == _lyncClient.Self.Contact.Uri)
                            {
                                this.AddToContactList(_Self);
                            }
                            else
                            {
                                this.ContractList.Remove(new LyncContract(_Self, _dispatcher));
                            }
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
                }
            }), null);
        }
        

        void AddToContactList(Contact contact)
        {
            LyncContract contract = new LyncContract(contact, _dispatcher);

            if (!this.ContractList.Contains<LyncContract>(contract, LyncContractComparer.Default))
            {
                this.ContractList.Add(contract);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
