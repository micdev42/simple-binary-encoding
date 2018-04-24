﻿// Copyright (C) 2017 MarketFactory, Inc
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Org.SbeTool.Sbe.Tests
{
    [TestClass]
    public sealed class Issue483Tests
    {
        [TestMethod]
        public void PresenceTest()
        {
            // Check our attributes for their presence meta attribute
            Assert.AreEqual(Issue483.Issue483.UnsetMetaAttribute(Issue483.MetaAttribute.Presence), "required");
            Assert.AreEqual(Issue483.Issue483.RequiredMetaAttribute(Issue483.MetaAttribute.Presence), "required");
            Assert.AreEqual(Issue483.Issue483.ConstantMetaAttribute(Issue483.MetaAttribute.Presence), "constant");
            Assert.AreEqual(Issue483.Issue483.OptionalMetaAttribute(Issue483.MetaAttribute.Presence), "optional");
        }
    }
}
