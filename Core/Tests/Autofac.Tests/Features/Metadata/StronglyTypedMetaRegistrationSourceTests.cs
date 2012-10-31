﻿using System;
using System.ComponentModel.Composition;
using Autofac.Core;
using Autofac.Features.Metadata;
using NUnit.Framework;

namespace Autofac.Tests.Features.Metadata
{
    [TestFixture]
    public class StronglyTypedMeta_WhenMetadataIsSupplied
    {
        const int SuppliedValue = 123;
        IContainer _container;

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<object>().WithMetadata("TheInt", SuppliedValue);
            _container = builder.Build();
        }

        [Test]
        public void ValuesAreProvidedFromMetadata()
        {
            var meta = _container.Resolve<Meta<object, MyMeta>>();
            Assert.AreEqual((int) SuppliedValue, (int) meta.Metadata.TheInt);
        }

        [Test]
        public void ValuesProvidedFromMetadataOverrideDefaults()
        {
            var meta = _container.Resolve<Meta<object, MyMetaWithDefault>>();
            Assert.AreEqual((int) SuppliedValue, (int) meta.Metadata.TheInt);
        }

        [Test]
        public void ValuesBubbleUpThroughAdapters()
        {
            var meta = _container.Resolve<Meta<Func<object>, MyMeta>>();
            Assert.AreEqual((int) SuppliedValue, (int) meta.Metadata.TheInt);
        }
    }

    [TestFixture]
    public class StronglyTypedMeta_WhenNoMatchingMetadataIsSupplied
    {
        IContainer _container;

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<object>();
            _container = builder.Build();
        }

        [Test]
        public void ResolvingStronglyTypedMetadataWithoutDefaultValueThrowsException()
        {
            Assert.Throws<DependencyResolutionException>(() => _container.Resolve<Meta<object, MyMeta>>());
        }

        [Test]
        public void ResolvingStronglyTypedMetadataWithDefaultValueProvidesDefault()
        {
            var m = _container.Resolve<Meta<object, MyMetaWithDefault>>();
            Assert.AreEqual((int) 42, (int) m.Metadata.TheInt);
        }
    }
}