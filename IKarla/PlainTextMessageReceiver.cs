using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using Takenet.MessagingHub.Client.Extensions.Directory;
using Lime.Messaging.Contents;
using System.Text.RegularExpressions;
using Sone.Chatbots.Sac;

namespace IKarla
{
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IStateManager _stateManager;
        private readonly IDirectoryExtension _directory; //para pegar os dados do cara sem ter que perguntar 

        private readonly string _greetings = "Iniciar|Começar|start|bom dia|boa tarde|boa noite|olá|Olá|e aí|E aí|e ai|E ai|colé|Colé|fala comigo|Fala comigo|wtp|Wtp|what's up|opa|joia|What's up|Opa|Joia";
        private readonly string _compliments = "bonita|engraçada|inteligente|divertida|esperta|prestativa";
        private readonly string _dismissal = "tchau|vlw|até mais";
        private readonly string _whatToAsk = "Então, o que você gostaria de saber? ⬇";
        private readonly string _whatElse = "O que mais você gostaria de saber? ⬇";

        public PlainTextMessageReceiver(IMessagingHubSender sender, IStateManager stateManager, IDirectoryExtension directory)
        {
            _sender = sender;
            _stateManager = stateManager;
            _directory = directory;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var user = await _directory.GetDirectoryAccountAsync(message.From, cancellationToken);
            var text = message.Content.ToString().ToLower();

            if (text.Contains("vc"))
                text = text.Replace("vc", "você");

            if (text.Contains("oq "))
                text = text.Replace("oq", "o que");

            if (text.Contains("rsrs") || text.Contains("haha") || text.Contains("huahsu"))
                return;

            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");

            if (text.StartsWith("oi") || text.StartsWith("ola") || Regex.IsMatch(text, _greetings))
            {
                await _sender.SendMessageAsync($@"Olá, {user.FullName.Split(' ')[0]}! Seja bem-vindo(a).", message.From, cancellationToken);
                Thread.Sleep(3000);
                await _sender.SendMessageAsync("Eu sou a Alice, assistente pessoal da Karla, prazer em conhecê-la(o).", message.From, cancellationToken);
                Thread.Sleep(3000);
                await _sender.SendMessageAsync("Ah, eu sou um bot e estou aqui para responder suas perguntas sobre a Karla.", message.From, cancellationToken);
                Thread.Sleep(3000);
                await _sender.SendMessageAsync("O que eu não souber responder, pode deixar que eu pergunto pra ela e você pode voltar pra me perguntar depois. 😉", message.From, cancellationToken);
                Thread.Sleep(3000);

                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (Regex.IsMatch(text, _compliments))
            {
                await _sender.SendMessageAsync("Obrigada! ☺", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (Regex.IsMatch(text, _dismissal))
            {
                await _sender.SendMessageAsync("Até mais! Volte sempre! 😘", message.From, cancellationToken);
                return;
            }

            if (text.Contains("obrigad"))
            {
                var op = new Random();

                switch (op.Next(1, 3))
                {
                    case 1:
                        await _sender.SendMessageAsync("Por nada! 😜", message.From, cancellationToken);
                        break;
                    case 2:
                        await _sender.SendMessageAsync("Imagina! 😄", message.From, cancellationToken);
                        break;
                    case 3:
                        await _sender.SendMessageAsync("Anytime! 😉", message.From, cancellationToken);
                        break;
                }

                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
                return;
            }

            if (Regex.IsMatch(text, "quem é karla"))
            {
                await _sender.SendMessageAsync("Essa mocinha aí que está palestrando...", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (Regex.IsMatch(text, "o que você faz") || Regex.IsMatch(text, "pra que você serve") || Regex.IsMatch(text, "ajuda"))
            {
                await _sender.SendMessageAsync("Eu respondo perguntas sobre a Karla.", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (Regex.IsMatch(text, "o que é um bot?") || Regex.IsMatch(text, "bot?"))
            {
                await _sender.SendMessageAsync("É só uma abreviação para 'robot', robô em inglês. 😄", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (Regex.IsMatch(text, "quantos anos") && Regex.IsMatch(text, "tem") || Regex.IsMatch(text, "idade"))
            {
                if (text.Contains("você") || text.Contains("sua"))
                {
                    await _sender.SendMessageAsync("Bom, tecnicamente, eu tenho 1 mês de vida... Não sei quanto é isso em anos de robôs.", message.From, cancellationToken);
                }
                else
                {
                    await _sender.SendMessageAsync("A Karla tem 24 anos.", message.From, cancellationToken);
                }

                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (Regex.IsMatch(text, "seu nome"))
            {
                await _sender.SendMessageAsync("Meu nome é Alice. 😃", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (!Regex.IsMatch(text, "você sabe") && (Regex.IsMatch(text, "você") || Regex.IsMatch(text, "sua") || Regex.IsMatch(text, "seu")))
            {
                var op = new Random();

                switch (op.Next(1,3))
                {
                    case 1:
                        await _sender.SendMessageAsync("Eu não gosto muito de falar sobre mim. Vamos falar da Karla?", message.From, cancellationToken);
                        break;
                    case 2:
                        await _sender.SendMessageAsync("Prefiro falar sobre a Karla, o que acha?", message.From, cancellationToken);
                        break;
                    case 3:
                        await _sender.SendMessageAsync("Pensei que estivéssimos aqui para falar da Karla...", message.From, cancellationToken);
                        break;
                }
                
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (text.Contains("trabalha"))
            {
                await _sender.SendMessageAsync("Atualmente a Karla trabalha na Solutions One como Analista de Sistemas.", message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("Solutions One"))
            {
                var s1 = new WebLink()
                {
                    Title = "A Solutions One é uma empresa que fornece soluções, especialmente, para o mercado de seguros, assistências e outros serviços de valor agregado. Para saber mais sobre a S1, entra nesse site aqui. 😄",
                    Uri = new Uri("https://site.solutionsone.com.br/"),
                    PreviewUri = new Uri("https://thumb.lovemondays.com.br/cPruf6SrX5DJFyYv1I3Rmy8x_9E=/102x102/https://media.lovemondays.com.br/logos/a57c7e/solutions-one-original.png"),
                    PreviewType = "image/png"
                };
                await _sender.SendMessageAsync(s1, message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("idioma") || text.Contains("lingua") || text.Contains("língua"))
            {
                await _sender.SendMessageAsync("A Karla adora aprender novos idiomas! Atualmente ela fala Inglês e Francês, além de sua língua nativa, o Portugês. 🇧🇷 🇺🇸 🇫🇷 <3", message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("formação") || text.Contains("formaçao") || text.Contains("acadêmica") || text.Contains("academica") || text.Contains("graduação") || text.Contains("faculdade") || text.Contains("graduaçao"))
            {
                await _sender.SendMessageAsync("A Karla é Bacharel em Sistemas de Informação pela Faculdade Promove de Tecnologia. Ela concluiu parte da graduação dela estudando Ciência da Computação no Instituo de Tecnologia da Florida.", message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains(" cu ") || text.Contains("fuder") || text.Contains("foder") || text.Contains("fodas") || text.Contains("puta") || text.Contains("burra") || text.Contains("idiota"))
            {
                await _sender.SendMessageAsync("Hey! Calm your tits, eu sou só a estagiária. 😒", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            if (text.Contains("chata"))
            {
                await _sender.SendMessageAsync($@"Já ouvi isso antes... 😉", message.From, cancellationToken);
                await DisplayOptions(_whatToAsk, message.From, cancellationToken);
                return;
            }

            switch (text)
            {
                case "linkedin":
                    var linkedIn = new WebLink()
                    {
                        Title = "O LinkedIn dela é esse aqui!",
                        Uri = new Uri("https://br.linkedin.com/in/karlafortes"),
                        PreviewUri = new Uri("https://media.licdn.com/mpr/mpr/shrinknp_200_200/AAEAAQAAAAAAAAjwAAAAJGFmNTVlYjZiLTE0YTktNDBlNy1hYWNkLTRmMDc3NWE1ZTUwZg.jpg"),
                        PreviewType = "image/png"
                    };
                    await _sender.SendMessageAsync(linkedIn, message.From, cancellationToken);
                    await DisplayOptions(_whatElse, message.From, cancellationToken);
                    return;

                case "instagram":
                    var instagram = new WebLink
                    {
                        Title = "O insta dela é esse aqui! Segue lá! 😄",
                        Uri = new Uri("https://www.instagram.com/karlacfortes/"),
                        PreviewUri = new Uri("https://instagram.fplu3-1.fna.fbcdn.net/t51.2885-19/s150x150/18646284_280321172371552_6262224392453881856_a.jpg"),
                        PreviewType = "image/png"
                    };
                    await _sender.SendMessageAsync(instagram, message.From, cancellationToken);
                    await DisplayOptions(_whatElse, message.From, cancellationToken);
                    return;

                case "food":
                    var food = new MediaLink
                    {
                        Title = "A comida favorita dela é lasanha. 😍 (Mas ela não dispensa um bom pedaço de picanha! 😉)",
                        Uri =  new Uri("http://oladobomdavida.com/wp-content/uploads/2016/11/garfield-invade-pagina-do-facebook-em-busca-de-10-mil-lasanhas-em-campanha-divertida-1.jpg"),
                        Type = new MediaType(MediaType.DiscreteTypes.Image, MediaType.SubTypes.Bitmap)
                    };
                    await _sender.SendMessageAsync(food, message.From, cancellationToken);
                    await DisplayOptions(_whatElse, message.From, cancellationToken);
                    return;
            }

            if (text.Contains("nudes"))
            {
                var nudes = new MediaLink
                {
                    Title = "😏",
                    Uri = new Uri("https://nandasiepierskimakeup.files.wordpress.com/2016/05/tons-de-pele.png?w=300&h=225"),
                    Type = new MediaType(MediaType.DiscreteTypes.Image, MediaType.SubTypes.Bitmap)
                };
                await _sender.SendMessageAsync(nudes, message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("cor favorita"))
            {
                await _sender.SendMessageAsync("A cor favorita da Karla é preto.", message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("a karla odeia"))
            {
                await _sender.SendMessageAsync("Beterraba. 😰", message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("a karla ama"))
            {
                await _sender.SendMessageAsync("Um monte de coisa.", message.From, cancellationToken);
                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            if (text.Contains("sair comigo") || text.Contains("sairia comigo") || text.Contains("ficar comigo") || text.Contains("casar comigo"))
            {
                var boyfriend = new MediaLink
                {
                    Title = "Desculpa, mas esse aí é o namorado dela. Gato né?!",
                    Uri = new Uri("https://nandasiepierskimakeup.files.wordpress.com/2016/05/tons-de-pele.png?w=300&h=225"),
                    Type = new MediaType(MediaType.DiscreteTypes.Image, MediaType.SubTypes.Bitmap)
                };
                await _sender.SendMessageAsync(boyfriend, message.From, cancellationToken);

                var brother = new MediaLink
                {
                    Title = "Ah, e o irmão dela é meio bravo... ",
                    Uri = new Uri("https://nandasiepierskimakeup.files.wordpress.com/2016/05/tons-de-pele.png?w=300&h=225"),
                    Type = new MediaType(MediaType.DiscreteTypes.Image, MediaType.SubTypes.Bitmap)
                };
                await _sender.SendMessageAsync(brother, message.From, cancellationToken);

                await DisplayOptions(_whatElse, message.From, cancellationToken);
                return;
            }

            await _sender.SendMessageAsync("Desculpe, eu não sei responder isso. Mas vou procurar saber, tá?", message.From, cancellationToken);
            await DisplayOptions(_whatToAsk, message.From, cancellationToken);
        }

        public async Task DisplayOptions(string text, Node user, CancellationToken cancellationToken)
        {
            var options = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Title", "Qual o LinkedIn da Karla?" },
                    { "Uri", "https://www.coisasdeti.com.br/wp-content/uploads/2017/11/linkedin.png" },
                    { "Value", "linkedin" }
                },
                new Dictionary<string, string>
                {
                    { "Title", "A Karla tem instagram?" },
                    { "Uri", "https://lh3.googleusercontent.com/aYbdIM1abwyVSUZLDKoE0CDZGRhlkpsaPOg9tNnBktUQYsXflwknnOn2Ge1Yr7rImGk=w300" },
                    { "Value", "instagram" }
                },
                new Dictionary<string, string>
                {
                    { "Title", "Qual a comida favorita da Karla?" },
                    { "Uri", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR-qMglocWwafnCOTrFagSF4mVgmSANb8H-nBbeVvTHMbvsYT4q" },
                    { "Value", "food" }
                }
            };

            var headers = new string[options.Count][];
            var mediaOptions = new List<Dictionary<string, string>>();

            for (int i = 0; i < options.Count; i++)
            {
                headers[i] = new[]
                {
                    options[i]["Title"],
                    string.Empty, 
                    options[i]["Uri"]
                };

                
                mediaOptions.Add(new Dictionary<string, string>
                {
                    { "Quero saber isso!", options[i]["Value"]}
                });
            }

            Thread.Sleep(8000);
            await _sender.SendMessageAsync(text, user, cancellationToken);

            var displayOptions = new DocumentCollection
            {
                ItemType = DocumentSelect.MediaType,
                Items = new CarrouselCards().GetCarrouselCards(headers, mediaOptions),
                Total = 3
            };

            await _sender.SendMessageAsync(displayOptions, user, cancellationToken);
        }
    }
}
