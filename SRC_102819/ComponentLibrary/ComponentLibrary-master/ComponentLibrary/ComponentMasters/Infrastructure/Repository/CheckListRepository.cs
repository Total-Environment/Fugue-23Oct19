using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <inheritdoc/>
    public class CheckListRepository : ICheckListRepository
    {
        private readonly IMongoRepository<CheckListDao> _mongoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListRepository"/> class.
        /// </summary>
        /// <param name="mongoRepository">The mongo repository.</param>
        public CheckListRepository(IMongoRepository<CheckListDao> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        /// <inheritdoc/>
        public async Task<CheckList> Add(CheckList checkList)
        {
            var checkListDao = new CheckListDao(checkList);
            var existingCheckList = await _mongoRepository.FindBy("CheckListId", checkListDao.CheckListId);
            if (existingCheckList != null)
                throw new ArgumentException("CheckList already exists.");
            var insertedDao = await _mongoRepository.Add(checkListDao);
            return insertedDao.CheckList;
        }

        /// <inheritdoc/>
        public async Task<CheckList> GetById(string id)
        {
            var checkListDao = await _mongoRepository.FindBy("CheckListId", id);
            if (checkListDao == null)
                throw new ResourceNotFoundException("CheckList");
            return checkListDao?.CheckList;
        }
    }
}