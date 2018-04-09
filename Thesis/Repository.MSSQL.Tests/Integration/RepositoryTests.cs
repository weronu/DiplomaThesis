using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
using NUnit.Framework;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL.Tests.Integration
{
    public class RepositoryTests : IntegrationBase
    {

        //[Test]
        //public void User_RepositoryTest()
        //{
        //    User user;
        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        user = uow.CommonRepo.GetById<User>(2);
        //    }

        //    Assert.IsNotNull(user);
        //}

        //[Test]
        //public void EmailMessage_RepositoryTest()
        //{
        //    EmailMessage email;
        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        email = uow.CommonRepo.GetById<EmailMessage>(2);
        //    }

        //    Assert.IsNotNull(email);
        //}

        //[Test]
        //public void EmailRecipient_RepositoryTest()
        //{
        //    EmailRecipient recipient;
        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        recipient = uow.CommonRepo.GetById<EmailRecipient>(82573);
        //    }

        //    Assert.IsNotNull(recipient);
        //}

        //[Test]
        //public void GraphRepository_VericesTest()
        //{
        //    HashSet<Node<User>> vertices;

        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        vertices = uow.GraphRepo.ExtractNodesFromDatabase();
        //    }

        //    Assert.IsNotNull(vertices);
        //    Assert.IsNotEmpty(vertices);
        //}

        //[Test]
        //public void GraphRepository_EdgesTest()
        //{
        //    HashSet<Edge<User>> edges;

        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        edges = uow.GraphRepo.ExtractEdgesFromDatabase();
        //    }

        //    Assert.IsNotNull(edges);
        //    Assert.IsNotEmpty(edges);
        //}

        //[Test]
        //public void Conversations_Test()
        //{
        //    HashSet<ConversationEmails> conversationEmails;

        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        conversationEmails = uow.ConvRepo.ExtractConversationsFromDatabase();
        //    }

        //    Assert.IsNotNull(conversationEmails);
        //    Assert.IsNotEmpty(conversationEmails);
        //}

        //[Test]
        //public void GraphRepository_ConversationVertices_Test()
        //{
        //    HashSet<Node<User>> vertices;

        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        vertices = uow.GraphRepo.ExtractNodesFromConversations();
        //    }

        //    Assert.IsNotNull(vertices);
        //    Assert.IsNotEmpty(vertices);
        //}

        //[Test]
        //public void GraphRepository_ConversationEdges_Test()
        //{
        //    HashSet<Edge<User>> edges;

        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        edges = uow.GraphRepo.ExtractEdgesFromConversation();
        //    }

        //    Assert.IsNotNull(edges);
        //    Assert.IsNotEmpty(edges);
        //}
    }
}
