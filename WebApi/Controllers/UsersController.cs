﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;
using WebApi.UserService;
using Common;
using NLog;

namespace WebApi.Controllers
{
    // GetSubscriptionById all users (GET -> /users)
    //GetSubscriptionById current user (GET -> /users/some-url-friendly-identifier)
    //Create user (POST -> /users)
    //Add subscriptions to user (PUT -> /users/subscriptionId)
    //Delete user (DELETE -> /users/some-url-friendly-identifier)

    public class UsersController : ApiController
    {
        private Logger log;
        private IUserRepo repo;
        public UsersController()
        {
            UserServiceClient client = new UserServiceClient();
            repo = new UserRepo(client);
            log = LogManager.GetCurrentClassLogger();
        }
        //some-url-friendly-identifier = int 
        //GetSubscriptionById current user (GET -> /users/some-url-friendly-identifier)
        [Route("Users/{id}")]
        [HttpGet]
        public ApiUser GetUserById(int id)
        {
            var user = repo.GetUser(id);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return user;

        }

        //GetSubscriptionById all users (GET -> /users)
        [Route("Users")]
        [HttpGet]
        public IEnumerable<ApiUser> GetUsers()
        {
            log.Debug("WebApi GetSubscriptionById()");
            return repo.GetUsers();
        }
        [Route("Users")]
        [HttpPost]
        public HttpResponseMessage CreateUser([FromBody]ApiUser user)
        {
            try
            {
                var newApiUser = repo.CreateUser(user);
                return Request.CreateResponse<ApiUser>(HttpStatusCode.Created, newApiUser);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //Add subscriptions to user (PUT -> /users/subscriptionId) spec changed to:
        //Add subscriptions to user (PUT -> /users/userId) (major violation of spec)
        [Route("Users")]
        [HttpPut]
        public HttpResponseMessage UpdateUser(ApiUser user)
        {
            try
            {
                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentNullException("ApiUser"));
                }
                //get CurrentUser instead
                user = repo.UpdateUser(user);
                return Request.CreateResponse<ApiUser>(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.GetBaseException());
            }

        }

        //Delete user (DELETE -> /users/some-url-friendly-identifier)
        //Foreign Key with cascade on delete also deletes all of the users subscriptions.  ServiceBus ska till för detta
        [Route("Users")]
        [HttpDelete]
        public HttpResponseMessage DeleteUser(int id)
        {
            try
            {
                repo.DeleteUser(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}