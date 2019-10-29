import {apiUrl} from '../src/helpers';
import { Service } from '../src/components/service';
import {connect} from 'react-redux';
import moment from 'moment';
import axios from 'axios';
import {logException} from "./helpers";
import {Permissioned, permissions} from "./permissions/permissions";
import {getComponentViewPermissions, getViewServicePermissions} from "./permissions/ComponentPermissions";
import {logError, logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) =>
  ({serviceCode: props.params.serviceCode,
      details: state.reducer.components[props.params.serviceCode] && state.reducer.components[props.params.serviceCode].serviceDetails,
      classificationDefinition: state.reducer.components[props.params.serviceCode] && state.reducer.components[props.params.serviceCode].classificationDefinition,
      rates:state.reducer.serviceRates,
      error: state.reducer.error,
      rateserror:state.reducer.rateserror
  });

export const mapDispatchToProps = (dispatch) => ({
    onServiceFetchRequest: async (serviceCode) => {
    try {
      const response = await axios.get(apiUrl(`services/${serviceCode}`) + "?dataType=true");
      let serviceDetails = response.data;
      const classificationDefinition = serviceDetails.headers && serviceDetails.headers.find(header => header.key === 'classification_definition');
      let detailsWithoutClassificationDefinition = serviceDetails.headers && serviceDetails.headers.filter(header => header.key !== 'classification_definition');
      const details = {serviceDetails:Object.assign({},serviceDetails,{headers: detailsWithoutClassificationDefinition})
              ,classificationDefinition:classificationDefinition};
      logInfo('SERVICE_FETCH_SUCCEEDED' + '\n' + details);
      dispatch({type: 'SERVICE_FETCH_SUCCEEDED', details: details});
    } catch (error) {
      logException('SERVICE_FETCH_FAILED \n'+ error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'SERVICE_FETCH_FAILED',
          serviceCode,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'SERVICE_FETCH_FAILED',
          serviceCode,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({
          type: 'SERVICE_FETCH_FAILED',
          serviceCode,
          error: 'No Service with code: ' + serviceCode + ' is found.'
        });
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin."
        }
        dispatch({type: 'SERVICE_FETCH_FAILED', serviceCode, error: (error.response && error.response.data && error.response.data.message) || errorMessage});
      }
    }
  },
  onServiceRatesFetchRequest: async(serviceCode) => {
    try {
      const now = moment.utc().format();
      const response = await axios.get(apiUrl(`service-rates/latest?serviceId=${serviceCode}&on=${now}`));
      const serviceRates = response.data;
      logInfo('SERVICE_RATES_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'SERVICE_RATES_FETCH_SUCCEEDED', rates: serviceRates});
    } catch (error) {
      logException('SERVICE_RATES_FETCH_FAILED \n' + error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'SERVICE_RATES_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'SERVICE_RATES_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({
          type: 'SERVICE_RATES_FETCH_FAILED',
          error: 'No service Rates with code: ' + serviceCode + ' is found'
        });
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin."
        }
        dispatch({type: 'SERVICE_RATES_FETCH_FAILED', error:  (error.response && error.response.data && error.response.data.message) || errorMessage});
      }
    }
  },
  onServiceDestroy : ()=>{
    dispatch({type:'SERVICE_DESTROYED'});
  }
});

const component = connect(mapStateToProps, mapDispatchToProps)(Service);
export const ServiceConnector = Permissioned(component, getComponentViewPermissions('service'), true, null);
