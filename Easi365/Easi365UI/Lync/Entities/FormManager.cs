using System;
using System.Collections.Generic;
using Microsoft.Lync.Model.Conversation;

namespace Easi365UI.Lync
{
    public class FormManager
    {
        public static IList<ChatForm> Forms = new List<ChatForm>();

        public static ChatForm GetByID(Guid id)
        {
            foreach (var form in Forms)
            {
                if (form.ID == id)
                {
                    return form;
                }
            }

            return null;
        }

        public static ChatForm GetByContacts(IList<string> contacts)
        {
            foreach (var form in Forms)
            {
                if (!form.HasMultiParticipants && contacts.IsEqual(form.Contacts))
                {
                    return form;
                }
            }

            return null;
        }

        public static ChatForm GetByConversation(Conversation conversation)
        {
            var conversationId = (string)conversation.Properties[ConversationProperty.Id];
            foreach (var form in Forms)
            {
                if (form.ConversationId == conversationId)
                    return form;
            }

            return null;
        }

        public static void Add(ChatForm chatForm)
        {
            if (GetByContacts(chatForm.Contacts) == null)
            {
                Forms.Add(chatForm);
            }
        }


        public static void Remove(Guid id)
        {
            ChatForm form = GetByID(id);
            if (form != null)
            {
                Forms.Remove(form);
            }
        }
    }

    public class ChatForm
    {
        public ChatForm()
        {

        }

        public ChatForm(Guid id, IList<string> contacts, Chat chat)
        {
            this.ID = id;
            this.Chat = chat;
            this.Contacts = contacts;
        }

        public Guid ID { get; set; }

        public string ConversationId { get; set; }

        public IList<string> Contacts { get; set; }

        public bool HasMultiParticipants { get; set; }

        public Chat Chat { get; set; }
    }
}
