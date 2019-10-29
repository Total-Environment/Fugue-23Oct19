import {Material} from './components/material';
import {connect} from 'react-redux';
import {apiUrl} from '../src/helpers';
import moment from 'moment'
import axios from 'axios';
import {logException} from "./helpers";
import {Permissioned, permissions} from "./permissions/permissions";
import {getComponentViewPermissions, getMaterialViewPermissions} from "./permissions/ComponentPermissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => {
  return {
    materialCode: props.params.materialCode,
    details: state.reducer.components[props.params.materialCode],
    rates: state.reducer.currentMaterialRates,
    detailserror: state.reducer.newMaterialError,
    rateserror: state.reducer.rateserror,
    rentalRates: state.reducer.currentRentalRates,
    rentalRatesError: state.reducer.rentalRatesError
  }
};

export const mapDispatchToProps = (dispatch) => ({
  onMaterialFetchRequest: async(materialCode) => {
    try {
      const response = await axios.get(apiUrl(`materials/${materialCode}`) + '?dataType=true');
      logInfo('MATERIAL_FETCH_SUCCEEDED \n '+ response.data);
      dispatch({type: 'MATERIAL_FETCH_SUCCEEDED', details: response.data});

    } catch (error) {
      logException('MATERIAL_FETCH_FAILED \n ' + error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'MATERIAL_FETCH_FAILED',
          materialCode,
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'MATERIAL_FETCH_FAILED', materialCode, detailserror: 'No Material with code: ' + materialCode + ' is found.'});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin."
        }
        dispatch({type: 'MATERIAL_FETCH_FAILED', materialCode, detailserror: errorMessage});
      }
    }
  },
  onMaterialRatesFetchRequest: async(materialCode) => {
    try {
      const now = moment.utc().format();
      const response = await axios.get(apiUrl(`material-rates/latest?materialId=${materialCode}&on=${now}`));
      const materialRates = response.data;
      logInfo('MATERIAL_RATES_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'MATERIAL_RATES_FETCH_SUCCEEDED', rates: materialRates});
    }
    catch (error) {
      logException('MATERIAL_RATES_FETCH_FAILED \n '+ error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'MATERIAL_RATES_FETCH_FAILED',
        rateserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
      });
    }
    else if (error.response && error.response.status === 404) {
      dispatch({type: 'MATERIAL_RATES_FETCH_FAILED', rateserror: 'Material Rates are not found'});
    }
    else {
      let errorMessage = error.message;
      if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin."
      }
      dispatch({type: 'MATERIAL_RATES_FETCH_FAILED', rateserror: (error.response && error.response.data && error.response.data.message) || errorMessage});
    }
  }
  },
  onRentalRatesFetchRequest: async(materialCode) => {
    try {
      const now = moment.utc().format();
      const response = await axios.get(apiUrl(`materials/${materialCode}/rental-rates/active?appliedFrom=${now}`));
      const rentalRates = response.data;
      logInfo('RENTAL_RATES_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'RENTAL_RATES_FETCH_SUCCEEDED', rentalRates: rentalRates});
    } catch (error) {
      logException('RENTAL_RATES_FETCH_FAILED \n' + error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'RENTAL_RATES_FETCH_FAILED',
          rentalRatesError: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'RENTAL_RATES_FETCH_FAILED', rentalRatesError: 'Rental Rates are not found'});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin."
        }
        dispatch({type: 'RENTAL_RATES_FETCH_FAILED', rentalRatesError:(error.response && error.response.data && error.response.data.message) || errorMessage});
      }
    }
  },
  onMaterialDestroy: () => {
    dispatch({type: 'MATERIAL_DESTROYED'});
  }
});

const component = connect(mapStateToProps, mapDispatchToProps)(Material);
export const MaterialConnector = Permissioned(component,getComponentViewPermissions('material'),true,null);
