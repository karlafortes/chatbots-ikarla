using System;
using System.Collections.Generic;
using System.Linq;
using Lime.Messaging.Contents;
using Lime.Protocol;

namespace Sone.Chatbots.Sac
{
    public class CarrouselCards
    {
        public CarrouselCards()
        { }

        public Document[] GetCarrouselCards(string[][] headers = null, IDictionary<string, string> mediaOptions = null, IDictionary<string, string> webLinkOptions = null)
        {
            if (headers != null && !headers.Any()) return null;

            var cards = new DocumentSelect[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                DocumentContainer header = null;
                DocumentSelectOption[] options = null;
                if (headers.Any())
                {
                    header = GetHeader(headers[i][0], headers[i][1], headers[i][2]);
                }

                if (mediaOptions != null && mediaOptions.Any())
                {
                    options = GetOptions(mediaOptions);
                }

                if (webLinkOptions != null && webLinkOptions.Any())
                {
                    options = GetWebLinkOptions(webLinkOptions);
                }

                cards[i] = new DocumentSelect
                {
                    Header = header,
                    Options = options
                };
            }

            return cards;
        }

        public Document[] GetCarrouselCards(string[][] headers = null, List<Dictionary<string, string>> mediaOptions = null, List<Dictionary<string, string>> webLinkOptions = null)
        {
            if (headers != null && !headers.Any()) return null;

            var cards = new DocumentSelect[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                DocumentContainer header = null;
                DocumentSelectOption[] options = null;
                if (headers.Any())
                {
                    header = GetHeader(headers[i][0], headers[i][1], headers[i][2]);
                }

                if (mediaOptions != null && mediaOptions.Any())
                {
                    options = GetOptions(mediaOptions[i]);
                }

                if (webLinkOptions != null && webLinkOptions.Any())
                {
                    options = GetWebLinkOptions(webLinkOptions[i]);
                }

                cards[i] = new DocumentSelect
                {
                    Header = header,
                    Options = options
                };
            }

            return cards;
        }

        public Document[] GetCarrouselCards(string[] headers = null, IDictionary<string, string> mediaOptions = null, IDictionary<string, string> webLinkOptions = null)
        {
            if (headers != null && !headers.Any()) return null;

            var cards = new DocumentSelect[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                DocumentContainer header = null;
                DocumentSelectOption[] options = null;
                if (headers.Any())
                {
                    header = GetPlainTextHeader(headers[i]);
                }

                if (mediaOptions != null && mediaOptions.Any())
                {
                    options = GetOptions(mediaOptions);
                }

                if (webLinkOptions != null && webLinkOptions.Any())
                {
                    options = GetWebLinkOptions(webLinkOptions);
                }

                cards[i] = new DocumentSelect
                {
                    Header = header,
                    Options = options
                };
            }

            return cards;
        }

        private static DocumentContainer GetHeader(string title = null, string text = null, string image = null)
        {
            return new DocumentContainer
            {
                Value = new MediaLink
                {
                    Title = title ?? " ",
                    Text = text ?? " ",
                    Uri = image != null ? new Uri(image) : default(Uri),
                    Type = new MediaType(MediaType.DiscreteTypes.Image, MediaType.SubTypes.Bitmap)
                }
            };
        }

        private static DocumentContainer GetPlainTextHeader(string text = null)
        {
            return new DocumentContainer
            {
                Value = new PlainText
                {
                    Text = text ?? " "
                }
            };
        }

        private static DocumentSelectOption[] GetOptions(IDictionary<string, string> mediaOptions)
        {
            var count = mediaOptions.Count;

            if (count <= 0) return null;

            var labels = mediaOptions.Keys.ToArray();
            var values = mediaOptions.Values.ToArray();

            var options = new DocumentSelectOption[count];

            for (int i = 0; i < count; i++)
            {
                options[i] = new DocumentSelectOption
                {
                    Label = new DocumentContainer
                    {
                        Value = new PlainText
                        {
                            Text = labels[i]
                        }
                    },
                    Value = new DocumentContainer
                    {
                        Value = new PlainText
                        {
                            Text = values[i]
                        }
                    }
                };
            }

            return options;
        }

        private static DocumentSelectOption[] GetWebLinkOptions(IDictionary<string, string> webLinkOptions)
        {
            var count = webLinkOptions.Count;

            if (count <= 0) return null;

            var titles = webLinkOptions.Keys.ToArray();
            var uris = webLinkOptions.Values.ToArray();

            var options = new DocumentSelectOption[count];

            for (int i = 0; i < count; i++)
            {
                options[i] = new DocumentSelectOption
                {
                    Label = new DocumentContainer
                    {
                        Value = new WebLink
                        {
                            Title = titles[i],
                            Uri = new Uri(uris[i]),
                            Target = WebLinkTarget.SelfCompact
                        }
                    }
                };
            }

            return options;
        }
    }
}
