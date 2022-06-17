//
// Copyright 2020 Google LLC
//
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
//

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Solutions.Common.Test;
using Google.Solutions.IapDesktop.Application.Services.Adapters;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Google.Solutions.IapDesktop.Application.Test.Services.Authorization
{
    [TestFixture]
    public class TestUserInfo : ApplicationFixtureBase
    {
        //---------------------------------------------------------------------
        // TryGetPictureAsync.
        //---------------------------------------------------------------------

        [Test]
        public async Task WhenPictureIsNull_ThenTryGetPictureAsyncReturnsNull()
        {
            var userInfo = new UserInfo();
            var picture = await userInfo
                .TryGetPictureAsync(CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNull(picture);
        }

        [Test]
        public async Task WhenPicturePointsToNonexistingResource_ThenTryGetPictureAsyncReturnsNull()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.gstatic.com/generate_404"
            };
            var picture = await userInfo
                .TryGetPictureAsync(CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNull(picture);
        }

        [Test]
        public async Task WhenPicturePointsToOtherData_ThenTryGetPictureAsyncReturnsNull()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.gstatic.com/ipranges/goog.txt"
            };
            var picture = await userInfo
                .TryGetPictureAsync(CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNull(picture);
        }

        [Test]
        public async Task WhenPictureIsValid_ThenTryGetPictureAsyncReturnsPicture()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.google.com/images/errors/robot.png"
            };
            var picture = await userInfo
                .TryGetPictureAsync(CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNotNull(picture);
        }
    }
}
