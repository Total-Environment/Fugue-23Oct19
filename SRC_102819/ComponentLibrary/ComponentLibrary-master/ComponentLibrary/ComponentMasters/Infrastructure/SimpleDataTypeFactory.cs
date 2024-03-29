﻿using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure
{
    /// <summary>
    /// Represents a simple data type factory
    /// </summary>
    public class SimpleDataTypeFactory : ISimpleDataTypeFactory
    {
        private readonly ICheckListRepository _checkListRepository;
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IStaticFileRepository _staticFileRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDataTypeFactory"/> class.
        /// </summary>
        /// <param name="checkListRepository">The check list repository.</param>
        /// <param name="masterDataRepository">The master data repository.</param>
        /// <param name="staticFileRepository">The static file repository.</param>
        public SimpleDataTypeFactory(ICheckListRepository checkListRepository, IMasterDataRepository masterDataRepository, IStaticFileRepository staticFileRepository)
        {
            _checkListRepository = checkListRepository;
            _masterDataRepository = masterDataRepository;
            _staticFileRepository = staticFileRepository;
        }

        /// <inheritdoc/>
        public async Task<ISimpleDataType> Construct(string dataType, object subType)
        {
            switch (dataType)
            {
                case "String":
                    return new StringDataType();

                case "Constant":
                    return new ConstantDataType(subType.ToString());

                case "MasterData":
                    var masterDataList = await _masterDataRepository.Find((string)subType);
                    return new MasterDataDataType(masterDataList);

                case "Int":
                    return new IntDataType();

                case "Boolean":
                    return new BooleanDataType();

                case "Decimal":
                    return new DecimalDataType();

                case "Date":
                    return new DateDataType();

                case "Unit":
                    return new UnitDataType(subType.ToString());

                case "Array":
                    return new ArrayDataType((IDataType)subType);

                case "CheckList":
                    return new CheckListDataType(_checkListRepository);

                case "StaticFile":
                    return new StaticFileDataType(_staticFileRepository);

                case "Autogenerated":
                    return new AutogeneratedDataType((string)subType);

                case "Range":
                    return new RangeDataType((string)subType);

                case "Money":
                    return new MoneyDataType();

                default:
                    throw new ArgumentException($"{dataType} data type is not defined.");
            }
        }
    }
}