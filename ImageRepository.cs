using Penguin.Cms.Repositories;
using Penguin.Messaging.Core;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Security.Abstractions.Extensions;
using Penguin.Security.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Penguin.Cms.Images.Repositories
{
    public partial class ImageRepository : AuditableEntityRepository<Image>
    {
        private const string URL_EMPTY_MESSAGE = "Url can not be null or whitespace";
        protected ISecurityProvider<Image> SecurityProvider { get; set; }

        public ImageRepository(IPersistenceContext<Image> dbContext, ISecurityProvider<Image> securityProvider = null, MessageBus messageBus = null) : base(dbContext, messageBus)
        {
            this.SecurityProvider = securityProvider;
        }

        public System.Collections.Generic.IEnumerable<Image> Get(List<string> tags = null)
        {
            List<Image> allImages = this.ToList();

            if (tags != null)
            {
                allImages = allImages.Where(i =>
                {
                    foreach (string thisTag in tags)
                    {
                        if (!i.Tags.Any(t => string.Equals(t.Trim(), thisTag.Trim(), System.StringComparison.OrdinalIgnoreCase)))
                        {
                            return false;
                        }
                    }

                    return true;
                }).ToList();
            }

            return allImages;
        }

        public IEnumerable<Image> GetByUri(string Uri)
        {
            if (string.IsNullOrWhiteSpace(Uri))
            {
                throw new System.ArgumentException(URL_EMPTY_MESSAGE, nameof(Uri));
            }

            List<Image> allImages = this.ToList().Where(i => i.Uri.StartsWith(Uri, System.StringComparison.OrdinalIgnoreCase)).Where(i => this.SecurityProvider.TryCheckAccess(i)).ToList();

            return allImages;
        }
    }
}