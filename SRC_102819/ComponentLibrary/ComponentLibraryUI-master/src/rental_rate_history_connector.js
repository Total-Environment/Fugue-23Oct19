import {apiUrl} from '../src/helpers';
import {RentalRateHistory} from './components/rental-rate-history/index';
import {connect} from 'react-redux';
import axios from 'axios';
import querystring from "querystring";
import {browserHistory} from 'react-router';
import {logException} from "./helpers";
import {fetchMasterDataByNameIfNeeded} from "./actions";
import {bindActionCreators} from "redux";
import {getViewRentalRatePermission} from "./permissions/ComponentPermissions";
import {Permissioned} from "./permissions/permissions";
import {logError, logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => ({
  materialCode: props.params.materialCode,
  rentalRateHistory: state.reducer.rentalRateHistory[props.params.materialCode] || {isFetching:false,values:undefined},
  error: state.reducer.error,
  addRentalRateError:state.reducer.addRentalRateError||undefined,
  rentalRateAdding: state.reducer.rentalRateAdding||false,
  sortOrder: props.location.query.sortOrder,
  sortColumn: props.location.query.sortColumn,
  rentalUnit: props.location.query.rentalUnit,
  appliedFrom: props.location.query.appliedFrom,
  rentalUnitData: state.reducer.masterDataByName['rental_unit'],
  currencyData: state.reducer.masterDataByName['currency'],
});

export const mapDispatchToProps = (dispatch, ownProps) => ({
  onRentalRateHistoryFetchRequest: async(materialCode, pageNumber, sortColumn, sortOrder, rentalUnit, appliedFrom) => {
    try {
      dispatch({type: 'RENTAL_RATE_HISTORY_FETCH_STARTED', materialCode});
      const params = { pageNumber, sortColumn, sortOrder, rentalUnit, appliedFrom};
      const url = `materials/${materialCode}/rental-rates/all`;
      const response = await axios.get(apiUrl(url), {params});
      const rentalRateHistory = {'code':materialCode,'data': response.data};
      logInfo('RENTAL_RATE_HISTORY_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'RENTAL_RATE_HISTORY_FETCH_SUCCEEDED', rentalRateHistory: rentalRateHistory});
    }
    catch (error) {
      logException('RENTAL_RATE_HISTORY_FETCH_FAILED \n' +error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',
          materialCode: materialCode,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',
          materialCode: materialCode,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
       });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',  materialCode: materialCode, error: 'No rental rate history is found.'});
      }
      else {
        dispatch({type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',  materialCode: materialCode, error: error.message});
      }
    }
  },
  onAddRentalRate: async(materialId, rate)=>{
    try{
      dispatch({type: 'ADD_RENTAL_RATE_REQUESTED'});
      await axios.post(apiUrl(`/materials/${materialId}/rental-rates`), rate);
      dispatch({type: 'ADD_RENTAL_RATE_SUCCEEDED'});
      window.location.reload();
    } catch (error) {
      logException('ADD_RENTAL_RATE_FAILED \n' + error);
      let errorMsg=undefined;
      if(error.response.status==409){
        const FormattedDate = new Date(rate.appliedFrom);
        const FormattedDateString = FormattedDate.getDate()+"/"+(FormattedDate.getMonth() + 1).toString()+"/"+FormattedDate.getFullYear();
        errorMsg = `Duplicate entries are not allowed. A rental rate version on '${FormattedDateString}' for '${rate.unitOfMeasure}' already exists`;
      }
      if(!errorMsg){
        errorMsg = error.response ? error.response.data.message : error.message;
      }
      dispatch({type: 'ADD_RENTAL_RATE_FAILED', error: errorMsg});
    }
  },
  onAddRentalRateErrorClose: ()=>{
    dispatch({type:'ADD_RENTAL_RATE_ERROR_CLOSED'})
  },
  onAmendQueryParams: (params) => {
    const newParams = Object.assign({}, ownProps.location.query, params);
    browserHistory.push(`/materials/${ownProps.params.materialCode}/rental-rate-history?${querystring.stringify(newParams)}`);
    dispatch({type: 'RENTAL_RATE_HISTORY_DESTROY', materialCode: ownProps.params.materialCode});
  },
  ...bindActionCreators({fetchMasterDataByNameIfNeeded}, dispatch),
});

const component = connect(mapStateToProps, mapDispatchToProps)(RentalRateHistory);
export const RentalRateHistoryConnector = Permissioned(component, getViewRentalRatePermission(), true,null);
