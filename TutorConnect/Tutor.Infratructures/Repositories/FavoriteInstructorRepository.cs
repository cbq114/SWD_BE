using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.UserModel;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class FavoriteInstructorRepository : IFavoriteRepository
    {
        private readonly TutorDBContext _dbContext;

        public FavoriteInstructorRepository(TutorDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddFavoriteInstructor(string userName, string instructorId)
        {
            var favoriteInstructor = await _dbContext.FavoriteInstructors
                .Include(fi => fi.FavoriteInstructorDetails)
                .FirstOrDefaultAsync(fi => fi.UserName == userName);
            if (favoriteInstructor == null)
            {
                favoriteInstructor = new FavoriteInstructors
                {
                    UserName = userName,
                    FavoriteInstructorDetails = new List<FavoriteInstructorDetails>()
                };
                await _dbContext.FavoriteInstructors.AddAsync(favoriteInstructor);
            }

            if (favoriteInstructor.FavoriteInstructorDetails.Any(fid => fid.tutor == instructorId))
            {
                return false;
            }

            favoriteInstructor.FavoriteInstructorDetails.Add(new FavoriteInstructorDetails
            {
                tutor = instructorId
            });
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserViewInstructorModel>> GetFavoriteInstructors(string userName)
        {
            var favoriteInstructor = await _dbContext.FavoriteInstructors
                 .Include(fi => fi.FavoriteInstructorDetails)
                 .FirstOrDefaultAsync(fi => fi.UserName == userName);

            if (favoriteInstructor == null)
            {
                return new List<UserViewInstructorModel>();
            }

            var favoriteDetails = favoriteInstructor.FavoriteInstructorDetails
                .Select(fid => fid.tutor)
                .ToList();

            var instructors = await _dbContext.Users
                .Where(u => favoriteDetails.Contains(u.UserName))
                .Select(u => new UserViewInstructorModel
                {
                    UserName = u.UserName,
                    RoleId = u.RoleId,
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    DOB = u.DOB,
                    Address = u.Profile.Address,
                    TeachingExperience = u.Profile.TeachingExperience,
                    Education = u.Profile.Education,
                    Price = u.Profile.Price,
                    LanguageId = u.Profile.LanguageId,
                    Country = u.Profile.Country
                })
                .ToListAsync();

            return instructors;
        }


        public async Task InitializeFavoriteInstructors(string userName)
        {
            var favoriteInstructors = new FavoriteInstructors
            {
                UserName = userName
            };
            await _dbContext.FavoriteInstructors.AddAsync(favoriteInstructors);
        }

        public async Task<bool> RemoveFavoriteInstructor(string userName, string instructorId)
        {
            var favoriteInstructor = await _dbContext.FavoriteInstructors
                .Include(fi => fi.FavoriteInstructorDetails)
                .FirstOrDefaultAsync(fi => fi.UserName == userName);

            if (favoriteInstructor == null)
            {
                return false;
            }

            var instructorToRemove = favoriteInstructor.FavoriteInstructorDetails
                .FirstOrDefault(fid => fid.tutor == instructorId);

            if (instructorToRemove == null)
            {
                return false;
            }

            favoriteInstructor.FavoriteInstructorDetails.Remove(instructorToRemove);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
