// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Author:                  Joe Audette
// Created:                 2016-04-01
// Last Modified:           2016-07-25
// 


using cloudscribe.Syndication.Models.Rss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudscribe.Syndication.Web.Controllers
{
    [Route("api/[controller]")]
    public class RssController : Controller
    {
        public RssController(
            ILogger<RssController> logger,
            IEnumerable<IChannelProvider> channelProviders = null,
            IChannelProviderResolver channelResolver = null,
            IXmlFormatter xmlFormatter = null
            )
        {
            log = logger;
            this.channelProviders = channelProviders ?? new List<IChannelProvider>();
            if (channelProviders is List<IChannelProvider>)
            {
                var list = channelProviders as List<IChannelProvider>;
                if (list.Count == 0)
                {
                    list.Add(new NullChannelProvider());
                }
            }

            this.channelResolver = channelResolver ?? new DefaultChannelProviderResolver();
            this.xmlFormatter = xmlFormatter ?? new DefaultXmlFormatter();

        }

        private ILogger log;
        private IChannelProviderResolver channelResolver;
        private IEnumerable<IChannelProvider> channelProviders;
        private IChannelProvider currentChannelProvider;
        private IXmlFormatter xmlFormatter;

        [HttpGet]
        [ResponseCache(CacheProfileName = "RssCacheProfile")]
        public async Task<IActionResult> Index()
        {
            currentChannelProvider = channelResolver.GetCurrentChannelProvider(channelProviders);

            if (currentChannelProvider == null)
            {
                Response.StatusCode = 404;
                return new EmptyResult();
            }
            
            var currentChannel = await currentChannelProvider.GetChannel();

            if (currentChannel == null)
            {
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            if (ShouldRedirect(currentChannel, HttpContext))
            {
                Response.Redirect(currentChannel.RemoteFeedUrl, false);
            }

            var xml = xmlFormatter.BuildXml(currentChannel);

            return new XmlResult(xml);

        }

        private bool ShouldRedirect(RssChannel channel, HttpContext context)
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
