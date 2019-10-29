import {Sfg} from './components/sfg';
import {connect} from 'react-redux';
import {apiUrl} from '../src/helpers';
import axios from 'axios';
import R from 'ramda';
import moment from "moment-timezone";
import {onSfgFetchRequest} from "./actions";
import {Permissioned, permissions} from "./permissions/permissions";
import {getComponentViewPermissions} from "./permissions/ComponentPermissions";
import {logError, logInfo} from "./sumologic-logger";
import {logException} from "./helpers";


export const mapStateToProps = (state, props) => {
  return {
    sfgCode: props.params.sfgCode,
    details: state.reducer.components[props.params.sfgCode],
    error: state.reducer.error,
    cost: state.reducer.sfgCosts[props.params.sfgCode],
    sfgCostError: state.reducer.sfgCostError,
    sfgError: (state.reducer.components[props.params.sfgCode] || {}).error,
  }
};

export const mapDispatchToProps = (dispatch, ownProps) => ({
  onSfgFetchRequest: code => onSfgFetchRequest(code)(dispatch),
  onSfgDestroy: () => {
    dispatch({type: 'SFG_DESTROY'});
  },
  onSfgCostFetchRequest: async (location = "Bangalore", date = moment.utc().format()) => {
    try {
      const response = await axios.get(apiUrl(`sfgs/${ownProps.params.sfgCode}/cost`) + `?location=${location}&appliedOn=${date}`);
      const data = response.data;
      logInfo('SFG_COST_FETCH_SUCCEEDED\n' + response.data);
      dispatch({type: 'SFG_COST_FETCH_SUCCEEDED', cost: data, sfgCode: ownProps.params.sfgCode});
    }
    catch (error) {
      logException('SFG_COST_FETCH_FAILED \n' + error);
      if(error.response && error.response.status === 404) {
        dispatch({type: 'SFG_COST_FETCH_FAILED', error: error.response && error.response.data});
      }
      else if(error.response && error.response.status === 400) {
        dispatch({type: 'SFG_COST_FETCH_FAILED', error: error.response.data.message});
      }
     else {
        dispatch({
          type: 'SFG_COST_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      console.log(error);
    }
  },
  onSfgCostDestroy: () => {
    dispatch({type: 'SFG_COST_DESTROY', sfgCode: ownProps.params.sfgCode})
  },
});

const component = connect(mapStateToProps, mapDispatchToProps)(Sfg);
export const SfgConnector = Permissioned(component, getComponentViewPermissions('sfg'), true, null);
