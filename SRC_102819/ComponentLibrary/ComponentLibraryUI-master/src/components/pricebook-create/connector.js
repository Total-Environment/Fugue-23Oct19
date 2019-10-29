import {PriceBookCreate} from "./index";
import {connect} from "react-redux";
import axios from 'axios';
import { apiUrl, logException, uploadStaticFiles } from '../../helpers';
import {alertAsync, confirmAsync} from '../dialog';
import {toastr} from 'react-redux-toastr';
import moment from 'moment-timezone';
import {bindActionCreators} from "redux";
import {Permissioned, permissions} from "../../permissions/permissions";
import {getCreateCPRPermissions} from "../../permissions/ComponentPermissions";

export const mapStateToProps = (state, props) => ({
  classificationData: state.reducer.dependencyDefinitions || null,
  classificationLevels: state.reducer.classificationLevels || null,
  projects: state.reducer.projects || null,
  onCloseModal: props.onCloseCrateCprModal,
  callingComponent: props.callingComponent
});

export const mapDispatchToProps = (dispatch, ownProps) => ({
  fetchDependencyDefinition: async (componentType) => {
    let dependencyDefinitionId = `${componentType}Classifications`;
    try {
      dispatch({
        type: 'DEPENDENCY_DEFINITION_FETCH_REQUESTED',
        dependencyDefinitionId
      });
      const response = await axios.get(apiUrl(`dependency/${dependencyDefinitionId}?compressed=true`));
      const dependencyDefinition = response.data;
      dispatch({
        type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED',
        dependencyDefinitionId,
        dependencyDefinition
      });
    }
    catch (error) {
      let errorMessage = error.message;
      if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',dependencyDefinitionId, error: errorMessage});
      }
      else {
        alertAsync('Error', `Failed to fetch ${componentType} classification.`);
        logException(error);
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error
        });
      }
    }
  },
  onAddCpr: async (jsonToPost) => {
    const promise = new Promise(async (resolve, reject) => {
      try {
        dispatch({type: 'CREATE_CPR_REQUESTED'});
        const response = await axios.post(apiUrl('costpriceratios'), jsonToPost);
        let appliedDate = new Date(jsonToPost.appliedFrom);
        dispatch({type: 'CREATE_CPR_SUCCEEDED'});
        if(jsonToPost.code != null) {
          toastr.success('success', 'CPR successfully created for ' +jsonToPost.componentType + ' with code ' + jsonToPost.code +
                                    ' with applicable date ' + appliedDate.toDateString());
        }
        else {
          toastr.success('success', 'CPR successfully created for ' + jsonToPost.componentType +
                       ' for group ' + jsonToPost.level1 + `${jsonToPost.level2 != null ? ', '+jsonToPost.level2 : ''} ` +
                         `${jsonToPost.level3 != null ? ', '+jsonToPost.level3 : ''} ` +
                       ' with applicable date ' + appliedDate.toDateString());
        }
        ownProps.onCreateCpr();
        resolve();
      }
      catch (error) {
        if (error.response && error.response.data && error.response.data.message) {
          alertAsync('Error', error.response.data.message);
        }
        else {
          alertAsync('Error', error.message);
        }
        logException(error);
        dispatch({type: 'CREATE_CPR_FAILED'});
        reject();
      }
    });
    return promise;
  },
  onGetComponentDetails: async (componentType,componentCode) => {
    try {
      dispatch({type:'GET_COMPONENT_DETAILS_REQUESTED'});
      const response = await axios.get(apiUrl(componentType+'/'+componentCode));
      let classificationLevels = {componentType:componentType,levels:response.data.headers[0].columns.slice(0,3)};
      dispatch({type:'GET_COMPONENT_DETAILS_SUCCEEDED',
                classificationLevels: classificationLevels});
    }
    catch (error) {
      if(error.response.status === 404) {
        alertAsync('Error', 'Invalid component code, please enter a valid component code or use the classification level values to setup CPR at a group rather than a component');
      }
      else {
        alertAsync('Error', error.message);
      }
      logException(error);
      dispatch({type:'GET_COMPONENT_DETAILS_FAILED'});
    }
  },
  onFetchProjects: async() => {
    try {
      dispatch({type: 'FETCH_PROJECTS_REQUESTED'});
      const response = await axios.get(apiUrl('projects'));
      dispatch({type: 'FETCH_PROJECTS_SUCCEEDED', projects: response.data});
    } catch(e) {
      alertAsync('Error', 'Failed to fetch project exceptions');
      logException(e);
      dispatch({type: 'FETCH_PROJECTS_FAILED'});
    }
  },
  onDestroyComponentDetails: async (componentType,componentCode) => {
    dispatch({type: 'DESTROY_COMPONENT_DETAILS'});
  }
});

const component = connect(mapStateToProps, mapDispatchToProps)(PriceBookCreate);
export const PriceBookCreateConnector = Permissioned(component,getCreateCPRPermissions, true,null);
