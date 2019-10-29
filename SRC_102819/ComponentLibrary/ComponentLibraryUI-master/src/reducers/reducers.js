import { initialState } from '../initial-state';
import { updateIn } from "../helpers";

const getExtraField = (rate, rates, componentType) => {
  const extraKey = { material: 'materialName', service: 'shortDescription' }[componentType];
  const matchingRate = rates.find(r => r[`${componentType}Rate`].id === rate.id);
  const extraValue = matchingRate && matchingRate[extraKey];
  return { [extraKey]: extraValue };
};

function getErroredRates(state, { componentType, records, status }) {
  const rates = state.rates[componentType];
  return {
    [componentType]: records.filter(record => record.status === "Error")
      .map(record => ({ [`${componentType}Rate`]: record.recordData[0], ...getExtraField(record.recordData[0], rates, componentType) }))
  };
}
export function reducer(state = initialState, action) {
  function ComponentDefinitionFetchSuccess() {
    const clonedState = JSON.parse(JSON.stringify(state));
    clonedState.definitions = clonedState.definitions || {};
    clonedState.definitions[action.definition.name] = action.definition;
    return clonedState;
  }

  function ComponentBrandDefinitionFetchSuccess() {
    const clonedState = JSON.parse(JSON.stringify(state));
    clonedState.brandDefinitions = clonedState.brandDefinitions || {};
    clonedState.brandDefinitions[action.brandDefinition.name] = action.brandDefinition;
    return clonedState;
  }

  function GenericDefinitionFailed() {
    const clonedState = JSON.parse(JSON.stringify(state));
    clonedState.genericDefinition = null;
    return clonedState;
  }

  function MasterDataSuccess() {
    const clonedState = JSON.parse(JSON.stringify(state));
    if (!clonedState.masterData)
      clonedState.masterData = {};
    clonedState.masterData[action.masterData.id] = {
      status: 'fetched',
      values: action.masterData.values
    };
    return clonedState;
  }

  function masterDataLoading() {
    const clonedState = JSON.parse(JSON.stringify(state));
    if (!clonedState.masterData)
      clonedState.masterData = {};
    clonedState.masterData[action.masterDataId] = {
      status: 'loading',
      values: []
    };
    return clonedState;
  }

  function dependencyDefinitionSuccess() {
    let clonedState = JSON.parse(JSON.stringify(state));
    if (!clonedState.dependencyDefinitions)
      clonedState.dependencyDefinitions = {};

    clonedState.dependencyDefinitions[action.dependencyDefinitionId] = clonedState.dependencyDefinitions[action.dependencyDefinitionId] || {};

    clonedState.dependencyDefinitions[action.dependencyDefinitionId].isFetching = false;
    clonedState.dependencyDefinitions[action.dependencyDefinitionId].values = action.dependencyDefinition;
    clonedState.dependencyDefinitionError[action.componentType] =  '';

    return clonedState;
  }

  function dependencyDefinitionRequested() {
    let clonedState = JSON.parse(JSON.stringify(state));
    if (!clonedState.dependencyDefinitions)
      clonedState.dependencyDefinitions = {};

    clonedState.dependencyDefinitions[action.dependencyDefinitionId] = clonedState.dependencyDefinitions[action.dependencyDefinitionId] || {};
    clonedState.dependencyDefinitions[action.dependencyDefinitionId] = {
      isFetching: true,
      values: {}
    };
    clonedState.dependencyDefinitionError[action.componentType] =  '';

    return clonedState;
  }

  function SearchResultFetchSuccess() {
    const clonedState = JSON.parse(JSON.stringify(state));
    clonedState.error = '';
    clonedState.componentType = action.componentType;
    clonedState.search = {
      values: action.details,
      isFetching: false,
      isFiltering: action.isFiltering,
      componentType: action.componentType
    };
    return clonedState;
  }

  function SearchResultFetchFailed() {
    const clonedState = JSON.parse(JSON.stringify(state));
    clonedState.error = action.error;
    clonedState.search = {
      values: undefined,
      isFetching: false,
      isFiltering: false,
      filterData: [],
      componentType: action.componentType,
      errorType: (action.error && action.error.type) || null
    };
    return clonedState;
  }

  switch (action.type) {
    case 'CHECKLIST_FETCH_SUCCEEDED':
      return Object.assign({}, state, { currentChecklistDetails: action.checklist }, { error: '' });
    case 'CHECKLIST_FETCH_FAILED':
      return Object.assign({}, state, { currentChecklistDetails: null }, { error: action.error });
    case 'MATERIAL_FETCH_SUCCEEDED':
      return Object.assign({}, state, { components: Object.assign({}, state.components, { [action.details.id]: action.details }) }, { newMaterialError: '' });
    case 'SERVICE_FETCH_SUCCEEDED':
      return Object.assign({}, state, { components: Object.assign({}, state.components, { [action.details.serviceDetails.id]: action.details }) }, { error: '' });
    case 'SERVICE_FETCH_FAILED':
      return Object.assign({}, state, { components: Object.assign({}, state.components, { [action.serviceCode]: null }) }, { error: action.error });
    case 'SERVICE_RATES_FETCH_SUCCEEDED':
      return Object.assign({}, state, { serviceRates: action.rates }, { rateserror: '' });
    case 'SERVICE_RATES_FETCH_FAILED':
      return Object.assign({}, state, { serviceRates: null }, { rateserror: action.error });
    case 'MATERIAL_FETCH_FAILED':
      return Object.assign({}, state, { components: Object.assign({}, state.components, { [action.materialCode]: null }) }, { newMaterialError: action.detailserror });
    case 'MATERIAL_RATES_FETCH_SUCCEEDED':
      return Object.assign({}, state, { currentMaterialRates: action.rates }, {rateserror: ''});
    case 'MATERIAL_RATES_FETCH_FAILED':
      return Object.assign({}, state, { currentMaterialRates: null }, { rateserror: action.rateserror });
    case 'RENTAL_RATES_FETCH_SUCCEEDED':
      return Object.assign({}, state, { currentRentalRates: action.rentalRates }, { rentalRatesError: null });
    case 'RENTAL_RATES_FETCH_FAILED':
      return Object.assign({}, state, { currentRentalRates: null }, { rentalRatesError: action.rentalRatesError });
    /* Rental Rate History */
    case 'RENTAL_RATE_HISTORY_FETCH_FAILED':
      const rentalRateFailed = JSON.parse(JSON.stringify(state));
      rentalRateFailed.rentalRateHistory[action.materialCode] = {
        isFetching: false,
        values: undefined,
        error: action.error
      };
      return rentalRateFailed;
    case 'RENTAL_RATE_HISTORY_DESTROY':
      const rentalRateDestroyed = JSON.parse(JSON.stringify(state));
      delete rentalRateDestroyed.rentalRateHistory[action.materialCode];
      return rentalRateDestroyed;
    case 'RENTAL_RATE_HISTORY_FETCH_SUCCEEDED':
      return Object.assign({}, state, {
        rentalRateHistory: Object.assign({}, state.rentalRateHistory, {
          [action.rentalRateHistory.code]: {
            isFetching: false,
            values: action.rentalRateHistory.data,
            error: undefined
          }
        })
      });
    case 'RENTAL_RATE_HISTORY_FETCH_STARTED':
      return Object.assign({}, state, {
        rentalRateHistory: Object.assign({}, state.rentalRateHistory, {
          [action.materialCode]: {
            isFetching: true,
            values: undefined,
            error: action.error
          }
        })
      });
    case 'RATE_HISTORY_FETCH_SUCCEEDED':
      return Object.assign({}, state, { rateHistory: action.details }, { rateHistoryError: action.error });
    case 'RATE_HISTORY_FETCH_FAILED':
      return Object.assign({}, state, { rateHistory: null }, { rateHistoryError: action.error });
    case 'COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED':
      return ComponentDefinitionFetchSuccess();
    case 'COMPONENT_BRAND_DEFINITION_FETCH_SUCCEEDED':
      return ComponentBrandDefinitionFetchSuccess();
    case 'COMPONENT_BRAND_DEFINITION_FETCH_FAILED':
      return Object.assign({}, state, { brandDefinitions: null }, { error: action.error });
    case 'COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED':
      return ComponentDefinitionFetchSuccess();
    case 'COMPONENT_DEFINITION_FETCH_FAILED':
      return GenericDefinitionFailed();
    case 'COMPONENT_MASTER_DATA_FETCH_LOADING':
      return masterDataLoading();
    case 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED':
      return MasterDataSuccess();
    case 'COMPONENT_MASTER_DATA_FETCH_FAILED':
      return Object.assign({}, state, { ComponentMasterDataError: action.error });
    case 'DEPENDENCY_DEFINITION_FETCH_REQUESTED':
      return dependencyDefinitionRequested();
    case 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED':
      return dependencyDefinitionSuccess();
    case 'DEPENDENCY_DEFINITION_FETCH_FAILED':
      return Object.assign({}, state,{ dependencyDefinitionError: Object.assign({}, state.dependencyDefinitionError, { [action.componentType]: action.error }) }, {dependencyDefinitions: Object.assign({}, state.dependencyDefinitions, {[action.dependencyDefinitionId]: {isFetching: false}})});
    case 'SEARCH_RESULT_FETCH_SUCCEEDED':
      return SearchResultFetchSuccess();
    case 'SEARCH_RESULT_FETCH_FAILED':
      return SearchResultFetchFailed();
    case 'SEARCH_RESULT_FETCH_STARTED':
      return Object.assign({}, state, {
        error: '',
        search: {
          values: undefined,
          isFetching: true,
          componentType: action.componentType
        },
        isFiltering: action.isFiltering,
        filterData: action.filterData
      });
    case 'MATERIAL_DESTROYED':
      return Object.assign({}, state, { currentMaterialDetails: null }, { currentMaterialRates: null }, { rateserror: '' });
    case 'SERVICE_DESTROYED':
      return Object.assign({}, state, { currentServiceDetails: null }, { serviceRates: null }, { rateserror: '' });
    case 'CANCEL_COMPONENT_ERROR' :
      return Object.assign({}, state, { componentCreateError: '' });
    case 'SEARCH_RESULT_DESTROYED':
      return Object.assign({}, state, { search: undefined }, { error: undefined });
    case 'FILTERS_FETCH_STARTED':
      return Object.assign({}, state, { filterData: action.filterData });
    case 'ADD_MATERIAL_FAILED':
      return Object.assign({}, state, { componentCreateError: action.error, componentAdding: false });
    case 'ADD_SERVICE_FAILED':
      return Object.assign({}, state, { componentCreateError: action.error, componentAdding: false });
    case 'ADD_COMPONENT_REQUESTED':
      return Object.assign({}, state, { componentAdding: true });
    case 'ADD_COMPONENT_SUCCEEDED':
      return Object.assign({}, state, { componentAdding: false });
    case 'UPDATE_MATERIAL_SUCCEEDED':
      return Object.assign({}, state, {
        components: Object.assign({}, state.components, { [action.id]: null }),
        componentUpdating: false
      });
    case 'UPDATE_MATERIAL_FAILED':
      return Object.assign({}, state, { componentUpdateError: action.error, componentUpdating: false });
    case 'UPDATE_MATERIAL_REQUESTED':
      return Object.assign({}, state, { componentUpdating: true });
    case 'UPDATE_SERVICE_SUCCEEDED':
      return Object.assign({}, state, {
        components: Object.assign({}, state.components, { [action.id]: null }),
        componentUpdating: false
      });
    case 'UPDATE_SERVICE_FAILED':
      return Object.assign({}, state, { componentUpdateError: action.error, componentUpdating: false });
    case 'UPDATE_SERVICE_REQUESTED':
      return Object.assign({}, state, { componentUpdating: true });
    case 'RESET_UPDATE_ERROR':
      return Object.assign({}, state, { componentUpdateError: '' });
    case 'ADD_RATE_REQUESTED':
      return Object.assign({}, state, { rateAdding: true });
    case 'ADD_RATE_SUCCEEDED':
      return Object.assign({}, state, { rateAdding: false });
    case 'ADD_RATE_FAILED':
      return Object.assign({}, state, { addRateError: action.error }, { rateAdding: false });
    case 'ADD_RATE_ERROR_CLOSED':
      return Object.assign({}, state, { addRateError: null });
    case 'ASSET_DEFINITION_FETCH_STARTED':
      return Object.assign({}, state, {
        assetDefinitions: Object.assign({}, state.assetDefinitions, {
          [action.group]: {
            values: null,
            isFetching: true
          }
        })
      });
    case 'ASSET_DEFINITION_FETCH_SUCCEEDED':
      return Object.assign({}, state, {
        assetDefinitions: Object.assign({}, state.assetDefinitions, {
          [action.definition.name]: {
            values: action.definition,
            isFetching: false
          }
        })
      });
    case 'ASSET_DEFINITION_FETCH_FAILED':
      const newState = JSON.parse(JSON.stringify(state));
      delete newState.assetDefinitions[action.group];
      return newState;
    case 'EXCHANGE_RATES_FETCH_SUCCEEDED':
      return Object.assign({}, state, { currentExchangeRates: action.exchangeRates }, { exchangeRatesError: '' });
    case 'EXCHANGE_RATES_FETCH_FAILED':
      return Object.assign({}, state, { currentExchangeRates: null }, { exchangeRatesError: action.exchangeRatesError });
    case 'EXCHANGE_RATES_DESTROYED':
      return Object.assign({}, state, { currentExchangeRates: null }, { exchangeRatesError: '' });
    case 'EXCHANGE_RATE_HISTORY_FETCH_SUCCEEDED':
      return Object.assign({}, state, { exchangeRateHistory: action.details }, { error: action.error });
    case 'EXCHANGE_RATE_HISTORY_DESTROYED':
      return Object.assign({}, state, { exchangeRateHistory: undefined }, { error: undefined });
    case 'EXCHANGE_RATE_HISTORY_FETCH_FAILED':
      return Object.assign({}, state, { exchangeRateHistory: null }, { error: action.error });
    case 'ADD_EXCHANGE_RATE_REQUESTED':
      return Object.assign({}, state, { exchangeRateAdding: true });
    case 'ADD_EXCHANGE_RATE_SUCCEEDED':
      return Object.assign({}, state, { exchangeRateAdding: false });
    case 'ADD_EXCHANGE_RATE_FAILED':
      return Object.assign({}, state, { addExchangeRateError: action.error }, { exchangeRateAdding: false });
    case 'ADD_EXCHANGE_RATE_ERROR_CLOSED':
      return Object.assign({}, state, { addExchangeRateError: null });
    case 'DESTROY_EXCHANGE_RATE_HISTORY':
      return Object.assign({}, state, { exchangeRateHistory: null }, { error: null });
    case 'DESTROY_RATE_HISTORY':
      return Object.assign({}, state, { rateHistory: null }, { rateHistoryError: null });
    case 'ADD_RENTAL_RATE_REQUESTED':
      return Object.assign({}, state, { rentalRateAdding: true });
    case 'ADD_RENTAL_RATE_SUCCEEDED':
      return Object.assign({}, state, { rentalRateAdding: false });
    case 'ADD_RENTAL_RATE_FAILED':
      return Object.assign({}, state, { addRentalRateError: action.error }, { rentalRateAdding: false });
    case 'ADD_RENTAL_RATE_ERROR_CLOSED':
      return Object.assign({}, state, { addRentalRateError: null });
    case 'MASTER_DATA_FOR_RENTAL_RATE_FETCH_SUCCEEDED':
      return Object.assign({}, state, { unitOfMeasureForRentalRate: action.unitOfMeasureForRentalRate });
    case 'MASTER_DATA_FOR_RENTAL_RATE_FETCH_FAILED':
      return Object.assign({}, state, { unitOfMeasureForRentalRateError: action.unitOfMeasureForRentalRateError });
    case 'BRAND_FETCH_SUCCEEDED':
      return Object.assign({}, state, {
        components: Object.assign({}, state.components,
          { [action.details.id]: action.details })
      }, { fetchBrandError: '' });
    case 'BRAND_FETCH_FAILED':
      return Object.assign({}, state, { components: Object.assign({}, state.components, { [action.materialCode]: null }) },
        { fetchBrandError: action.detailserror });
    case 'EDIT_BRAND_REQUESTED':
      return Object.assign({}, state, { isSavingBrand: true });
    case 'EDIT_BRAND_SUCCEEDED':
      return Object.assign({}, state, {
        components: Object.assign({}, state.components,
          { [action.details.id]: action.details })
      }, { newBrandError: '' }, { isSavingBrand: false }, { updatedBrand: true });
    case 'ADD_BRAND_REQUESTED':
      return Object.assign({}, state, { brandAdding: true });
    case 'ADD_BRAND_SUCCEEDED':
      return Object.assign({}, state, {
        components: Object.assign({}, state.components,
          { [action.details.id]: action.details })
      }, { newBrandError: '' }, { brandAdding: false }, { addedBrand: true });
    case 'EDIT_BRAND_FAILED':
      return Object.assign({}, state, { newBrandError: action.error }, { brandAdding: false }, { updatedBrand: false });
    case 'ADD_BRAND_FAILED':
      return Object.assign({}, state, { newBrandError: action.error }, { brandAdding: false });
    case 'BRAND_DEFINITION_FETCH_SUCCEEDED':
      return Object.assign({}, state, { brandDefinition: action.brandDefinition, brandDefinitionError: '' });
    case 'BRAND_DEFINITION_FETCH_FAILED':
      return Object.assign({}, state, { brandDefinition: null, brandDefinitionError: action.error });
    case 'RATES_FETCH_REQUESTED':
      return Object.assign({}, state, { rates: Object.assign({}, state.rates, { [action.componentType]: action.rates }) },
        { ratesError: Object.assign({}, state.ratesError, { [action.componentType]: '' }) }, { isFetchingRates: true });
    case 'RATES_FETCH_SUCCEEDED':
      return Object.assign({}, state, { rates: Object.assign({}, state.rates, { [action.componentType]: action.rates }) },
        { ratesError: Object.assign({}, state.ratesError, { [action.componentType]: '' }) }, { isFetchingRates: false });
    case 'RATES_FETCH_FAILED':
      return Object.assign({}, state, { rates: Object.assign({}, state.rates, { [action.componentType]: undefined }) }, { ratesError: Object.assign({}, state.ratesError, { [action.componentType]: action.ratesError }) }, { isFetchingRates: false });
    case 'SET_RATE_FILTERS':
      return Object.assign({}, state, { rateFilters: Object.assign({}, state.rateFilters, { [action.componentType]: action.filters }) });
    case 'RESET_RATE_FILTERS':
      return Object.assign({}, state, { rateFilters: Object.assign({}, state.rateFilters, { [action.componentType]: initialState.rateFilters[action.componentType] }) });
    case 'DESTROY_RATES':
      return Object.assign({}, state, {
        rates: Object.assign({}, state.rates, { [action.componentType]: undefined }),
        ratesError: {material: false, service: false}, bulkRateError: {}, ratesEditable: { material: false, service: false }
      });
    case 'MASTER_DATA_BY_NAME_FETCH_REQUESTED':
      return updateIn(state, { isFetching: true, values: [],  error: action.error }, 'masterDataByName', action.name);
    case 'MASTER_DATA_BY_NAME_FETCH_SUCCEEDED':
      return updateIn(state, { isFetching: false, values: action.masterData, error: action.error }, 'masterDataByName', action.name);
    case 'MASTER_DATA_BY_NAME_FETCH_FAILED':
      return updateIn(state, { isFetching: false, values: action.masterData, error: action.error }, 'masterDataByName', action.name);
    case 'LOADING_STARTED':
      return Object.assign({}, state, { loading: { status: true, message: action.message } });
    case 'LOADING_FINISHED':
      return Object.assign({}, state, { loading: { status: false, message: '' } });
    case 'SFG_FETCH_STARTED':
      return updateIn(state, { isFetching: true, values: {} }, 'components', action.code);
    case 'SFG_FETCH_SUCCEEDED':
      return updateIn(state, { isFetching: false, values: action.details }, 'components', action.details.code);
    case 'SFG_FETCH_FAILED':
      return Object.assign({}, state, {
        components: Object.assign({}, {
          [action.code]: {
            isFetching: false,
            error: action.error
          }
        })
      });
    case 'SFG_DESTROY':
      return Object.assign({}, state, { details: null }, { error: null }, { classificationDefinition: null });
    case 'BULK_EDIT_RESPONSE':
      const erroredRates = getErroredRates(state, action);
      return Object.assign({}, state, {
        rates: Object.assign({}, state.rates, erroredRates),
        bulkRateError: Object.assign({}, state.bulkRateError, { [action.componentType]: { errored: erroredRates[action.componentType].length, total: action.records.length } })
      });
    case 'BULK_EDIT_CLEAR':
      return Object.assign({}, state, {
        rates: Object.assign({}, state.rates, { [action.componentType]: undefined }),
        bulkRateError: Object.assign({}, state.bulkRates, { [action.componentType]: undefined })
      });

    case 'TOGGLE_EDIT_RATES':
      return Object.assign({}, state, { ratesEditable: Object.assign({}, state.ratesEditable, { [action.componentType]: true }) });
    case 'ENABLE_EDIT_RATES':
      return Object.assign({}, state, { ratesEditable: Object.assign({}, state.ratesEditable, { [action.componentType]: true }) });
    case 'DISABLE_EDIT_RATES':
      return Object.assign({}, state, { ratesEditable: Object.assign({}, state.ratesEditable, { [action.componentType]: false }) });
    case 'PACKAGE_DEFINITION_FETCH_REQUESTED':
      return updateIn(state, { isFetching: true, values: {} }, 'definitions', 'package');
    case 'PACKAGE_DEFINITION_FETCH_SUCCEEDED':
      return updateIn(state, { isFetching: false, values: action.definition }, 'definitions', 'package');
    case 'PACKAGE_DEFINITION_FETCH_FAILED':
      return updateIn(state, undefined, 'definitions', 'package');
    case 'SFG_DEFINITION_FETCH_REQUESTED':
      return updateIn(state, { isFetching: true, values: {} }, 'definitions', 'sfg');
    case 'SFG_DEFINITION_FETCH_SUCCEEDED':
      return updateIn(state, { isFetching: false, values: action.definition }, 'definitions', 'sfg');
    case 'SFG_DEFINITION_FETCH_FAILED':
      return updateIn(state, undefined, 'definitions', 'sfg');
    case 'SFG_COST_FETCH_SUCCEEDED':
      return Object.assign({}, state, { sfgCosts: Object.assign({}, state.sfgCosts, { [action.sfgCode]: action.cost }) });
    case 'SFG_COST_FETCH_FAILED':
      return Object.assign({}, state, { sfgCostError: action.error });
    case 'SFG_COST_DESTROY':
      return Object.assign({}, state, { sfgCosts: Object.assign({}, state.sfgCosts, { [action.sfgCode]: null }) }, { sfgCostError: null });
    case 'SAVE_COMPOSITE_DETAILS_REQUESTED':
      return Object.assign({}, state, {
        sfgError: null,
        isSaving: true
      });
    case 'SAVE_COMPOSITE_DETAILS_SUCCEEDED':
      return Object.assign({}, updateIn(state, { isFetching: false, values: action.details }, 'components', action.details.code), {
        sfgError: null,
        isSaving: false
      });
    case 'SAVE_COMPOSITE_DETAILS_FAILED':
      return Object.assign({},
        action.code ? updateIn(state, { isFetching: false, values: null }, 'components', action.code) : state, {
          sfgError: action.sfgError,
          isSaving: false
        });
    case 'PACKAGE_FETCH_STARTED':
      return updateIn(state, { isFetching: true, values: {} }, 'components', action.code);
    case 'PACKAGE_FETCH_SUCCEEDED':
      return updateIn(state, { isFetching: false, values: action.details }, 'components', action.details.code);
    case 'PACKAGE_FETCH_FAILED':
      return Object.assign({}, state, {
        components: Object.assign({}, {
          [action.code]: {
            isFetching: false,
            error: action.error
          }
        })
      });
    case 'PACKAGE_COST_FETCH_SUCCEEDED':
      return Object.assign({}, state, { packageCosts: Object.assign({}, state.packageCosts, { [action.packageCode]: action.cost }) });
    case 'PACKAGE_COST_FETCH_FAILED':
      return Object.assign({}, state, { packageCostError: action.error });
    case 'PACKAGE_COST_DESTROY':
      return Object.assign({}, state, { packageCosts: Object.assign({}, state.packageCosts, { [action.packageCode]: null }) }, { packageCostError: null });
    case 'FETCH_CPRS_REQUESTED':
      return {...state, cprs: {isFetching: true}, cprError: action.error};
    case 'FETCH_CPRS_SUCCEEDED':
      return {...state, cprs: {isFetching: false, values: action.cprs.costPriceRatioDtos}, cprError: action.error};
    case 'FETCH_CPRS_FAILED':
      return {...state, cprs: undefined, cprError: action.error};
    case 'FETCH_PROJECTS_REQUESTED':
      return {...state, projects: {isFetching: true}, projectsError: action.error};
    case 'FETCH_PROJECTS_SUCCEEDED':
      return {...state, projects: {isFetching: false, values: action.projects}, projectsError: action.error};
    case 'FETCH_PROJECTS_FAILED':
      return {...state, projects: undefined, projectsError: action.error};
    case 'CREATE_CPR_REQUESTED':
      return state;
    case 'CREATE_CPR_SUCCEEDED':
      return state;
    case 'CREATE_CPR_FAILED':
      return state;
    case 'GET_COMPONENT_DETAILS_REQUESTED':
      return state;
    case 'GET_COMPONENT_DETAILS_SUCCEEDED':
      return Object.assign({}, state, { classificationLevels: action.classificationLevels });
    case 'GET_COMPONENT_DETAILS_FAILED':
      return Object.assign({}, state, { classificationLevels: null });
    case 'DESTROY_COMPONENT_DETAILS':
      return Object.assign({}, state, { classificationLevels: null });
    case 'DESTROY_PRICEBOOK_DETAILS':
      return Object.assign({}, state, {projectsError: undefined, cprError: undefined, dependencyDefinitionError: {
        material :'',
        service: '',
        sfg: '',
        package: ''
      }} );
    default:
      return state;
  }
}
