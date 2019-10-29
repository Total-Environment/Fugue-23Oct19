import { RatesHome } from './components/rates-home';
import { connect } from 'react-redux';
import axios from 'axios';
import { apiUrl } from '../src/helpers';
import { browserHistory } from 'react-router';
import {toastr} from 'react-redux-toastr'; import querystring from 'querystring';
import {logException} from "./helpers";
import {bindActionCreators} from "redux";
import {fetchMasterDataByNameIfNeeded} from "./actions";
import {Permissioned} from "./permissions/permissions";
import {getBulkRatesViewPermissions} from "./permissions/ComponentPermissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => {
  const componentType = props.route.componentType;
  const filters = state.reducer.rateFilters[componentType];
  return {
    classificationData: (state.reducer.dependencyDefinitions && state.reducer.dependencyDefinitions[`${componentType}Classifications`]),
    isFetchingRates: state.reducer.isFetchingRates || false,
    filters: state.reducer.rateFilters[componentType],
    isFilterApplied: filters.length > 0,
    masterData: state.reducer.masterDataByName,
    masterDataError: state.reducer.masterDataError,
    rates: state.reducer.rates[componentType],
    ratesError: state.reducer.ratesError,
    fetchingMasterData: state.reducer.fetchingMasterData,
    componentType,
    editable: state.reducer.ratesEditable[componentType],
    bulkRateError: state.reducer.bulkRateError[componentType],
    currencyData: state.reducer.masterDataByName['currency'],
    typeOfPurchaseData: state.reducer.masterDataByName['type_of_purchase'],
    typeOfPurchaseError: state.reducer.masterDataByName['type_of_purchase'] && state.reducer.masterDataByName['type_of_purchase'].error,
    currencyError: state.reducer.masterDataByName['currency'] && state.reducer.masterDataByName['currency'].error,
    statusError: state.reducer.masterDataByName['status'] && state.reducer.masterDataByName['status'].error,
    locationError: state.reducer.masterDataByName['location'] && state.reducer.masterDataByName['location'].error,
  }
};

export const mapDispatchToProps = (dispatch, ownProps) => ({
  onDependencyDefinitionFetch: async (dependencyDefinitionId) => {
    try {
      dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_REQUESTED', dependencyDefinitionId});
      const response = await axios.get(apiUrl(`dependency/${dependencyDefinitionId}?compressed=true`));
      const dependencyDefinition = response.data;
      logInfo('DEPENDENCY_DEFINITION_FETCH_SUCCEEDED \n'+response.data);
      dispatch({ type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED', dependencyDefinitionId, dependencyDefinition });
    }
    catch (error) {
      logException('DEPENDENCY_DEFINITION_FETCH_FAILED \n'+error);
      dispatch({ type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', dependencyDefinitionId, error });
    }
  },
  onRatesFetchRequest: async (filters = []) => {
    try {
      dispatch({ type: 'RATES_FETCH_REQUESTED' });
      const response = await axios.post(apiUrl(`${ownProps.route.componentType}s/rates`), filters);
      const rates = response.data;
      logInfo('RATES_FETCH_SUCCEEDED \n' + response.data);
      dispatch({ type: 'RATES_FETCH_SUCCEEDED', rates: rates, componentType: ownProps.route.componentType });
    }
    catch (error) {
      logException('RATES_FETCH_FAILED \n' +error);
      let errorMessage = error.message;
      if (error.response && error.response.status === 404) {
        dispatch({type: 'RATES_FETCH_FAILED', ratesError: `rates are not found for ${ownProps.route.componentType}s`, componentType: ownProps.route.componentType });
      }
      else if (error.response && error.response.status === 400) {
        if(error.response.data && error.response.data.message.indexOf('No exchange rate is found for currency') !== -1) {
          dispatch({type: 'RATES_FETCH_FAILED',ratesError:error.response.data.message, componentType: ownProps.route.componentType });
        }
        else {
          dispatch({
            type: 'RATES_FETCH_FAILED',
            ratesError: `unable to get rates for ${ownProps.route.componentType}s`,
            componentType: ownProps.route.componentType
          });
        }
      }
      else if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'RATES_FETCH_FAILED', ratesError: errorMessage, componentType: ownProps.route.componentType} );
      }
      else {
        dispatch({
          type: 'RATES_FETCH_FAILED',
          ratesError: (error.response && error.response.data && error.response.data.message) || errorMessage,
          componentType: ownProps.route.componentType
        });
      }
    }
  },
  onAmendQueryParams: (params) => {
    const newParams = Object.assign({}, ownProps.location.query, params);
    browserHistory.push(`/rates/${ownProps.route.componentType}s?${querystring.stringify(newParams)}`);
    dispatch({type: 'DESTROY_RATES', componentType: ownProps.route.componentType});
  },
  onDestroyRates: () => {
    dispatch({type: 'DESTROY_RATES', componentType: ownProps.route.componentType});
  },
  onUpdateBulkEdit: async (rates) => {
    dispatch({type: 'LOADING_STARTED', message: "This will take a few minutes. Please don't close the browser till then."});
    try {
      const response = await axios.post(apiUrl(`${ownProps.route.componentType}s/rates/bulk-edit`), rates);
      if(response.data.status === 'Succeeded') {
        toastr.success('Success', `${response.data.records.length} new rate versions have been updated successfully`);
        logInfo('BULK_EDIT_RATES_SUCCESS \n '+response.data);
        dispatch({type: 'BULK_EDIT_CLEAR', componentType: ownProps.route.componentType});
        dispatch({type: 'DISABLE_EDIT_RATES', componentType: ownProps.route.componentType});
      } else {
        logInfo('FAILED_TO_UPDATE_SOME BULK_EDIT_RATES_SUCCESS \n '+response.data);
        dispatch({type: 'BULK_EDIT_RESPONSE', records: response.data.records, status: response.data.status, componentType: ownProps.route.componentType});
        dispatch({type: 'ENABLE_EDIT_RATES', componentType: ownProps.route.componentType});
      }
      dispatch({type: 'LOADING_FINISHED'});
    }
    catch(error) {
      logException('BULK_EDIT_RATES_FAILED \n' + error);
      dispatch({type: 'LOADING_FINISHED'});
    }
  },
  onBulkEditClear: () => {
    dispatch({type: 'BULK_EDIT_CLEAR', componentType: ownProps.route.componentType});
  },
  onResetFilter: () => {
    dispatch({type: 'RESET_RATE_FILTERS', componentType: ownProps.route.componentType});
  },
  onSetNewFilters: (componentType, filters) => {
    dispatch({type: 'SET_RATE_FILTERS', componentType, filters});
  },
  onEnableEdit: () => {
    dispatch({type: 'ENABLE_EDIT_RATES', componentType: ownProps.route.componentType});
  },
  onDisableEdit: () => {
    dispatch({type: 'DISABLE_EDIT_RATES', componentType: ownProps.route.componentType});
  },
  ...bindActionCreators({fetchMasterDataByNameIfNeeded},dispatch)

});

const component = connect(mapStateToProps, mapDispatchToProps)(RatesHome);
export const RatesHomeConnector = Permissioned(component, getBulkRatesViewPermissions, true, null);
