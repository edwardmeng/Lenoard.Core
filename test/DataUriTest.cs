﻿using System;
using System.Linq;

namespace Lenoard.Core.UnitTests
{
    public class DataUriTest
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseBase64DataUri()
        {
            const string contentString = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==";
            DataUri uri;
            Assert.True(DataUri.TryParse("data:image/png;charset=utf-8;base64," + contentString, out uri));
            Assert.NotNull(uri);
            Assert.Equal("image/png", uri.MediaType);
            Assert.Equal("utf-8", uri.Parameters["charset"]);
            Assert.Equal("base64", uri.Encoding);
            Assert.IsInstanceOf<Base64DataUri>(uri);
            Assert.True(Convert.FromBase64String(contentString).SequenceEqual(((Base64DataUri)uri).Content));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseSimpleBase64()
        {
            const string contentString = "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==";
            DataUri uri;
            Assert.True(DataUri.TryParse("data:;base64," + contentString, out uri));
            Assert.NotNull(uri);
            Assert.Equal("base64", uri.Encoding);
            Assert.Equal("text/plain", uri.MediaType);
            Assert.IsInstanceOf<Base64DataUri>(uri);
            Assert.True(Convert.FromBase64String(contentString).SequenceEqual(((Base64DataUri)uri).Content));

        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTextDataUri()
        {
            const string contentString = "<script>alert('hi');</script>";
            DataUri uri;
            Assert.True(DataUri.TryParse("data:text/html," + contentString, out uri));
            Assert.NotNull(uri);
            Assert.Equal("text/html", uri.MediaType);
            Assert.Equal(string.Empty, uri.Encoding);
            Assert.IsInstanceOf<TextDataUri>(uri);
            Assert.Equal(contentString, ((TextDataUri)uri).Content);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseSimpleDataUri()
        {
            const string contentString = "Hello, World!";
            DataUri uri;
            Assert.True(DataUri.TryParse("data:," + contentString, out uri));
            Assert.NotNull(uri);
            Assert.Equal("text/plain", uri.MediaType);
            Assert.Equal(string.Empty, uri.Encoding);
            Assert.IsInstanceOf<TextDataUri>(uri);
            Assert.Equal(contentString, ((TextDataUri)uri).Content);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseSimpleParameter()
        {
            const string contentString = "Hello, World!";
            DataUri uri;
            Assert.True(DataUri.TryParse("data:;charset=utf-8," + contentString, out uri));
            Assert.NotNull(uri);
            Assert.Equal("text/plain", uri.MediaType);
            Assert.Equal("utf-8", uri.Parameters["charset"]);
            Assert.Equal(string.Empty, uri.Encoding);
            Assert.IsInstanceOf<TextDataUri>(uri);
            Assert.Equal(contentString, ((TextDataUri)uri).Content);
        }
    }
}
