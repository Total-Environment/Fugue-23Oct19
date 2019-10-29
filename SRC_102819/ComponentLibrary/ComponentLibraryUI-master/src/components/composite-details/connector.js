import {connect} from 'react-redux';
import {CompositeDetails} from './index';
import {apiUrl, logException, uploadStaticFiles} from '../../helpers';
import axios from 'axios';
import componentDataTransformer from '../../middlewares/component';
import {fetchMasterDataByName, masterDataFetch, onSfgFetchRequest, onPackageFetchRequest} from "../../actions";
import {browserHistory} from 'react-router';
import * as R from 'ramda';
import {alertAsync} from "../dialog/index";
import {toastr} from 'react-redux-toastr';
import {Permissioned} from "../../permissions/permissions";
import {getCompositeComponentsPermissions} from "../../permissions/ComponentPermissions";

const componentTypes = {'sfg': 'SFG', 'package': 'Package'};
export const mapDispatchToProps = (dispatch) => {
  return {
    resetSearch: async () => {
      dispatch({
        type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
        details: null,
        componentType: null
      });
    },
    saveCompositeDetails: async (payload, type, primaryCode, compositeCode) => {
      try {
        dispatch({
          type: 'SAVE_COMPOSITE_DETAILS_REQUESTED',
          payload
        });
        let getPayloadToPost = async (payload) => {
          if (primaryCode) {
            return payload;
          }
          return await uploadStaticFiles(payload);
        };
        let payloadToPost = await getPayloadToPost(payload);
        let endpoint = `/${type}s${primaryCode ? `?fromService=${primaryCode}` : ''}`;
        if (compositeCode) {
          endpoint = `/${type}s/${compositeCode}`;
        }
        const compositionData = payload.componentComposition ? {componentComposition: payload.componentComposition} :
          {componentCoefficients: payload.componentCoefficients};
        let response = await axios[compositeCode ? 'put' : 'post'](apiUrl(endpoint), {
          headers: payloadToPost.jsonToPost && payloadToPost.jsonToPost.headers, ...compositionData
        });
        dispatch({
          type: 'SAVE_COMPOSITE_DETAILS_SUCCEEDED',
          details: response.data || null
        });
        browserHistory.push(`/${type}s/${response.data.code}`);
        toastr.success('Success', compositeCode ? `The ${componentTypes[type]} ${response.data.code} details are updated successfully` :  `The ${componentTypes[type]} ${response.data.code} is created successfully`);
        if (payloadToPost.listOfFailedStaticFiles && payloadToPost.listOfFailedStaticFiles.length > 0) {
          toastr.warning('Failed to upload ' + payload.listOfFailedStaticFiles.toString() +
            ' file(s).', 'Please retry uploading the file(s).');
        }
      } catch (error) {
        alertAsync('Error', (error.response &&
          error.response.data &&
          error.response.data.message) || `Failed to save ${componentTypes[type]} data.`);
        logException(error);
        dispatch({
          type: 'SAVE_COMPOSITE_DETAILS_FAILED',
          code: compositeCode,
          sfgError: error.response.data
        });
      }
    },
    searchComponents: async (componentType, pageNumber, group, filterDatas) => {
      let batchSize = 5;
      let endPoint = `${componentType === 'asset' ? 'material' : componentType}s/${componentType === 'sfg' ? 'search' : 'searchwithingroup'}`;
      try {
        dispatch({
          type: 'SEARCH_RESULT_FETCH_STARTED',
          isFiltering: filterDatas ? !!filterDatas.length : false,
          filterData: filterDatas,
          componentType
        });
        let response = await axios.post(apiUrl(endPoint), {
          filterDatas,
          groupName: group,
          pageNumber,
          batchSize,
          ignoreSearchQuery: true
        });

        dispatch({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: response.data,
          componentType
        });
      }
      catch (error) {
        logException(error);
        if (error.response && error.response.status === 404) {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}, componentType});
        } else if (error.response && error.response.status === 400) {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}, componentType});
        } else {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'Unknown', error}, componentType});
        }
      }
    },
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
        alertAsync('Error', `Failed to fetch ${componentType} classification.`);
        logException(error);
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error
        });
      }
    },
    fetchPackageDefinition: async () => {
      try {
        dispatch({type: 'PACKAGE_DEFINITION_FETCH_REQUESTED'});
        const {data} = await axios.get(apiUrl(`/package-definitions`));
        dispatch({type: 'PACKAGE_DEFINITION_FETCH_SUCCEEDED', definition: data});
      } catch (e) {
        alertAsync('Error', `Failed to fetch package definition.`);
        logException(e);
        dispatch({type: 'PACKAGE_DEFINITION_FETCH_FAILED'});
      }
    },
    fetchSfgDefinition: async () => {
      try {
        dispatch({type: 'SFG_DEFINITION_FETCH_REQUESTED'});
        const {data} = await axios.get(apiUrl(`/sfg-definitions`));
        dispatch({type: 'SFG_DEFINITION_FETCH_SUCCEEDED', definition: data});
      } catch (e) {
        alertAsync('Error', `Failed to fetch sfg definition.`);
        logException(e);
        dispatch({type: 'SFG_DEFINITION_FETCH_FAILED'});
      }
    },
    onSearchDestroy: () => {
      dispatch({type: 'SEARCH_RESULT_DESTROYED'});
    },
    onMasterDataFetch: (id) => masterDataFetch(id)(dispatch),
    onSfgFetchRequest: (id) => onSfgFetchRequest(id)(dispatch),
    onPackageFetchRequest: id => onPackageFetchRequest(id)(dispatch),
    fetchMasterDataByName: (name) => fetchMasterDataByName(name)(dispatch),
  };
};

export const mapStateToProps = (state, props) => {
  let compositeCode = (props.route.componentType === 'sfg' ?
      props.routeParams.sfgCode : props.routeParams.packageCode) || '';
  return {
    searchResults: state.reducer.search || null,
    statusData: state.reducer.masterDataByName['status'] || null,
    classificationData: state.reducer.dependencyDefinitions || null,
    classificationDataError: state.reducer.dependencyDefinitionError || null,
    isRequestingClassification: props.isRequestingClassification,
    mode: props.route.mode,
    componentType: props.route.componentType,
    definition: state.reducer.definitions[props.route.componentType],
    masterData: state.reducer.masterData,
    code: props.params.code,
    compositeCode,
    sfgData: state.reducer.components[compositeCode],
    sfgError: state.reducer.sfgError || (state.reducer.components[compositeCode] || {}).error,
    isSaving: state.reducer.isSaving || false
  };
};

const component = connect(mapStateToProps, mapDispatchToProps)(CompositeDetails);
export const CompositeDetailsConnector = Permissioned(component, getCompositeComponentsPermissions, true,null);
