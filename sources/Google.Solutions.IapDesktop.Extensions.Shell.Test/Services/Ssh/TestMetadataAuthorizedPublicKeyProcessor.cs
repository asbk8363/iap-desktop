﻿//
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

using Google.Apis.Compute.v1.Data;
using Google.Solutions.Common.ApiExtensions.Instance;
using Google.Solutions.Common.Locator;
using Google.Solutions.Common.Test;
using Google.Solutions.IapDesktop.Application.Services.Adapters;
using Google.Solutions.IapDesktop.Application.Services.Authorization;
using Google.Solutions.IapDesktop.Application.Test;
using Google.Solutions.IapDesktop.Extensions.Shell.Services.Ssh;
using Google.Solutions.Ssh.Auth;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Google.Solutions.IapDesktop.Extensions.Shell.Test.Services.Ssh
{
    [TestFixture]
    public class TestMetadataAuthorizedPublicKeyProcessor : ApplicationFixtureBase
    {

        //---------------------------------------------------------------------
        // AddPublicKeyToMetadata.
        //---------------------------------------------------------------------

        [Test]
        public void WhenMetadataHasExpiredKeys_ThenAddPublicKeyToMetadataRemovesExpiredKeys()
        {
            var expiredKey = new ManagedMetadataAuthorizedPublicKey(
                "alice-expired",
                "ssh-rsa",
                "KEY-ALICE",
                new ManagedMetadataAuthorizedPublicKey.PublicKeyMetadata(
                    "alice@example.com",
                    DateTime.Now.AddDays(-1)));

            var metadata = new Metadata();
            metadata.Add(
                MetadataAuthorizedPublicKeySet.MetadataKey,
                MetadataAuthorizedPublicKeySet
                    .FromMetadata(new Metadata())
                    .Add(expiredKey)
                    .ToString());

            MetadataAuthorizedPublicKeyProcessor.AddPublicKeyToMetadata(
                metadata,
                new ManagedMetadataAuthorizedPublicKey(
                    "alice-valid",
                    "ssh-rsa",
                    "KEY-ALICE",
                    new ManagedMetadataAuthorizedPublicKey.PublicKeyMetadata(
                        "alice@example.com",
                        DateTime.Now.AddDays(+1))));

            StringAssert.DoesNotContain("alice-expired", metadata.Items.First().Value);
            StringAssert.Contains("alice-valid", metadata.Items.First().Value);
        }

        //---------------------------------------------------------------------
        // RemovePublicKeyFromMetadata.
        //---------------------------------------------------------------------

        [Test]
        public void WhenMetadataHasExpiredKeys_ThenRemoveAuthorizedKeyLeavesThemAsIs()
        {
            var expiredKey = new ManagedMetadataAuthorizedPublicKey(
                "alice-expired",
                "ssh-rsa",
                "KEY-ALICE",
                new ManagedMetadataAuthorizedPublicKey.PublicKeyMetadata(
                    "alice@example.com",
                    DateTime.Now.AddDays(-1)));

            var validKey = new ManagedMetadataAuthorizedPublicKey(
                "alice-valid",
                "ssh-rsa",
                "KEY-ALICE",
                new ManagedMetadataAuthorizedPublicKey.PublicKeyMetadata(
                    "alice@example.com",
                    DateTime.Now.AddDays(+1)));

            var metadata = new Metadata();
            metadata.Add(
                MetadataAuthorizedPublicKeySet.MetadataKey,
                MetadataAuthorizedPublicKeySet
                    .FromMetadata(new Metadata())
                    .Add(expiredKey)
                    .Add(validKey)
                    .ToString());

            MetadataAuthorizedPublicKeyProcessor.RemovePublicKeyFromMetadata(
                metadata,
                validKey);

            StringAssert.Contains("alice-expired", metadata.Items.First().Value);
        }
    }
}