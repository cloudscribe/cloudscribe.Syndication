// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Author:                  Joe Audette
// Created:                 2016-04-01
// Last Modified:           2018-06-19
// 


using cloudscribe.Syndication.Models.Rss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudscribe.Syndication.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RssController : Controller
    {
        public RssController(
            ILogger<RssController> logger,
            IEnumerable<IChannelProvider> channelProviders = null,
            IChannelProviderResolver channelResolver = null,
            IXmlFormatter xmlFormatter = null
            )
        {
            Log = logger;
            ChannelProviders = channelProviders ?? new List<IChannelProvider>();
            if (channelProviders is List<IChannelProvider>)
            {
                var list = channelProviders as List<IChannelProvider>;
                if (list.Count == 0)
                {
                    list.Add(new NullChannelProvider());
                }
            }

            ChannelResolver = channelResolver ?? new DefaultChannelProviderResolver();
            XmlFormatter = xmlFormatter ?? new DefaultXmlFormatter();

        }

        protected ILogger Log { get; private set; }
        protected IChannelProviderResolver ChannelResolver { get; private set; }
        protected IEnumerable<IChannelProvider> ChannelProviders { get; private set; }
        protected IChannelProvider CurrentChannelProvider { get; private set; }
        protected IXmlFormatter XmlFormatter { get; private set; }

        [HttpGet]
        [ResponseCache(CacheProfileName = "RssCacheProfile")]
        [Route("api/rss")]
        public virtual async Task<IActionResult> Index()
        {
            CurrentChannelProvider = ChannelResolver.GetCurrentChannelProvider(ChannelProviders);

            if (CurrentChannelProvider == null)
            {
                Response.StatusCode = 404;
                return new EmptyResult();
            }
            
            var currentChannel = await CurrentChannelProvider.GetChannel();

            if (currentChannel == null)
            {
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            if (ShouldRedirect(currentChannel, HttpContext))
            {
                Response.Redirect(currentChannel.RemoteFeedUrl, false);
            }

            var xml = XmlFormatter.BuildXml(currentChannel);

            return new XmlResult(xml);

        }

        protected bool ShouldRedirect(RssChannel channel, HttpContext context)
        {
            if (string.IsNullOrEmpty(channel.RemoteFeedUrl)) return false;
            if (string.IsNullOrEmpty(channel.RemoteFeedProcessorUseAgentFragment)) return false;
               
            var userAgentHeaders = context.Request.Headers["User-Agent"];
            if (userAgentHeaders.Count == 0) return true;
                
            var userAgent = userAgentHeaders[0];
            if (string.IsNullOrEmpty(userAgent)) return true;
            
            if (userAgent.Contains(channel.RemoteFeedProcessorUseAgentFragment))
            {
                return false;
            }
               
            return true;

        }

    }
}
