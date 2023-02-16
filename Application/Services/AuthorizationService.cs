using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class AuthorizationService
    {
        private readonly IIdentityClaimRepository _identityClaimRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public AuthorizationService(ApplicationDbContext context, ICache cache)
        {
            _identityClaimRepository = new IdentityClaimRepository(context, cache);
            _applicationUserRepository = new ApplicationUserRepository(context, cache);
        }

        public bool IsIdentityClaimExist(string userId, string claimType, string claimValue) => _identityClaimRepository.IsIdentityClaimExist(userId, claimType, claimValue);

        public void AddIdentityClaim(IdentityClaim claim) => _identityClaimRepository.AddIdentityClaim(claim);

        public void UpdateIdentityClaim(IdentityClaim claim) => _identityClaimRepository.UpdateIdentityClaim(claim);

        public void DeleteIdentityClaim(string userId, string claimType, string claimValue) => _identityClaimRepository.DeleteIdentityClaim(userId, claimType, claimValue);

        public bool IsExistsUser(string userId) => _applicationUserRepository.IsHasUserById(userId);
    }
}
