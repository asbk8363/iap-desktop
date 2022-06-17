//
// Copyright 2022 Google LLC
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

using Google.Solutions.Common.Test.Integration;
using Google.Solutions.IapDesktop.Application.Services.Adapters;
using NUnit.Framework;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Google.Solutions.IapDesktop.Application.Test.Services.Adapters
{
    [TestFixture]
    public class TestSignInAvatarAdapter : ApplicationFixtureBase
    {
        //---------------------------------------------------------------------
        // TryGetAvatarAsync (unsized).
        //---------------------------------------------------------------------

        [Test]
        public async Task WhenPictureIsNull_ThenTryGetAvatarAsyncReturnsNull()
        {
            var userInfo = new UserInfo();
            var adapter = new SignInAvatarAdapter();

            var picture = await adapter
                .TryGetAvatarAsync(userInfo, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNull(picture);
        }

        [Test]
        public async Task WhenPicturePointsToNonexistingResource_ThenTryGetAvatarAsyncReturnsNull()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.gstatic.com/generate_404"
            };
            var adapter = new SignInAvatarAdapter();

            var picture = await adapter
                .TryGetAvatarAsync(userInfo, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNull(picture);
        }

        [Test]
        public async Task WhenPicturePointsToOtherData_ThenTryGetAvatarAsyncReturnsNull()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.gstatic.com/ipranges/goog.txt"
            };
            var adapter = new SignInAvatarAdapter();

            var picture = await adapter
                .TryGetAvatarAsync(userInfo, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNull(picture);
        }

        [Test]
        public async Task WhenPictureIsValid_ThenTryGetAvatarAsyncReturnsImage()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.google.com/images/errors/robot.png"
            };
            var adapter = new SignInAvatarAdapter();

            var picture = await adapter
                .TryGetAvatarAsync(userInfo, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNotNull(picture);
        }

        //---------------------------------------------------------------------
        // TryGetAvatarAsync (sized).
        //---------------------------------------------------------------------

        [Test]
        public async Task WhenSizeSpecified_ThenTryGetAvatarAsyncReturnsSizedImage()
        {
            var userInfo = new UserInfo()
            {
                Picture = "https://www.google.com/images/errors/robot.png"
            };
            var adapter = new SignInAvatarAdapter();

            var picture = await adapter
                .TryGetAvatarAsync(
                    userInfo, 
                    new Size(16, 16),
                    CancellationToken.None)
                .ConfigureAwait(false);

            Assert.IsNotNull(picture);
            Assert.AreEqual(16, picture.Width);
            Assert.AreEqual(16, picture.Height);
        }
    }
}
