import {CreateComponent} from './components/create-component';
import {connect} from 'react-redux';
import {apiUrl} from '../src/helpers';
import {browserHistory} from 'react-router';
import axios from 'axios';
import {toastr} from 'react-redux-toastr';
import {containerName, head, logException, updateColumn} from "./helpers";
import {Permissioned, permissions} from "./permissions/permissions";
import {getCreateComponentPermissions} from "./permissions/ComponentPermissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => ({
  componentType: props.componentType,
  definitions: state.reducer.definitions || {},
  masterData: state.reducer.masterData || {},
  dependencyDefinitions: state.reducer.dependencyDefinitions || {},
  assetDefinitions: state.reducer.assetDefinitions,
  componentCreateError: state.reducer.componentCreateError,
  componentAdding: state.reducer.componentAdding || false,
  dependencyDefinitionError: state.reducer.dependencyDefinitionError
});

export const uploadStaticFiles = async (details) => {
  let arrayOfStaticFiles = [], staticFiles = [], failedArrayOfStaticFiles = [], listOfFailedStaticFiles = [];
  let jsonToPost = {headers: []};
  details.headers
    .forEach(async (header) => {
      let columns = [];
      header.columns
      .forEach(async (column) => {
      column.value === '' ?
        columns.push({value:null,key: column.key, name: column.name}) :
        columns.push({value:column.value,
                key:column.key,
                name:column.name});
      if (column.dataType.name === "StaticFile" &&
        column.value !== null) {
        if (typeof(column.value) === "number") {
          const file = window.fileList[column.value][0];
          const formData = new FormData();
          formData.append('File', file);
          staticFiles.push({
            headerKey:header.key, columnKey:column.key, formData, columnName: column.name
          });
        }
      }
      if (column.dataType.name === "Array" &&
        column.dataType.subType.name === "StaticFile" &&
        column.value !== null) {
        const length = column.value.length;
        for (let i = 0; i < length; i++) {
          if (typeof(column.value[i]) === "number") {
            const file = window.fileList[column.value[i]][0];
            const formData = new FormData();
            formData.append('File', file);
            arrayOfStaticFiles.push({
              headerKey: header.key, columnKey:column.key, formData, index: i, columnName: column.name
            });
          }
        }
      }
    });
    jsonToPost.headers.push({columns,key: header.key,name: header.name});
  });
  await Promise.all(arrayOfStaticFiles
    .map(async (file) => {
      try {
        const response = await axios.post(apiUrl(`upload/${containerName}`), file.formData);
        let value = head(jsonToPost, file.headerKey, file.columnKey, 'value');
        value[file.index] = response.data[0];
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : value});
      }
      catch (error) {
        failedArrayOfStaticFiles
          .push({headerKey: file.headerKey, index: file.index, columnKey: file.columnKey,columnName: file.columnName});
        listOfFailedStaticFiles.push(file.columnName);
      }
    }));

  for (let i = 0; i < failedArrayOfStaticFiles.length; i++) {
    const staticFile = failedArrayOfStaticFiles[i];
    if (staticFile.index > -1 && head(jsonToPost,staticFile.headerKey,staticFile.columnKey,'value')) {
      let value = head(jsonToPost, staticFile.headerKey, staticFile.columnKey, 'value');
      value.splice(staticFile.index, 1);
      jsonToPost = updateColumn(jsonToPost,staticFile.headerKey,staticFile.columnKey,{value : value});
      if (value.length === 0) {
        jsonToPost = updateColumn(jsonToPost,staticFile.headerKey,staticFile.columnKey,{value: null});
      }
    }
  }

  await Promise.all(staticFiles
    .map(async (file) => {
      try {
        const response = await axios.post(apiUrl(`upload/${containerName}`), file.formData);
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : response.data[0]});
      }
      catch (error) {
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : null});
        listOfFailedStaticFiles.push(file.columnName);
      }
    }));
  return ({jsonToPost, listOfFailedStaticFiles});
};

export const mapDispatchToProps = (dispatch) => ({
  onMaterialDefinitionFetch: async (materialLevel2) => {
    try {
      const response = await axios.get(apiUrl(`material-definitions/${materialLevel2}`));
      const definition = response.data;
      logInfo('COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED\n'+response.data);
      dispatch({type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED', definition});
    } catch (error) {
      logException('COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED\n'+error);
      dispatch({type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED', error})
    }
  },

  onServiceDefinitionFetch: async (serviceLevel1) => {
    try {
      const response = await axios.get(apiUrl(`service-definitions/${serviceLevel1}`));
      const definition = response.data;
      logInfo('COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED\n'+response);
      dispatch({type: 'COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED', definition});
    } catch (error) {
      logException('COMPONENT_SERVICE_DEFINITION_FETCH_FAILED\n'+error);
      dispatch({type: 'COMPONENT_SERVICE_DEFINITION_FETCH_FAILED', error})
    }
  },
  onMasterDataFetch: async (masterDataId) => {
    try {
      dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_LOADING', masterDataId});
      const response = await axios.get(apiUrl(`master-data/${masterDataId}`));
      const masterData = response.data;
      logInfo('COMPONENT_MASTER_DATA_FETCH_SUCCEEDED\n'+response.data);
      dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED', masterData});
    } catch (error) {
      logException('COMPONENT_MASTER_DATA_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 404) {
        dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error: {type: 'NotFound'}});
      } else if (error.response && error.response.status === 400) {
        dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error: {type: 'NetworkError', error: 'Some thing went wrong. Problem has been recorded and will be fixed.'}});
      }
      else {
        dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error});
      }
    }
  },
  onDependencyDefinitionFetch: async (dependencyDefinitionId) => {
    try {
      const response = await axios.get(apiUrl(`dependency/${dependencyDefinitionId}?compressed=true`));
      const dependencyDefinition = response.data;
      logInfo('DEPENDENCY_DEFINITION_FETCH_SUCCEEDED\n'+response.data);
      dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED', dependencyDefinitionId, dependencyDefinition});
    }
    catch (error) {
      logException('DEPENDENCY_DEFINITION_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 404) {
        dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', error: {type: 'NotFound'}});
      } else if (error.response && error.response.status === 400) {
        dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', error: {type: 'NetworkError', error: (error.response && error.response.data && error.response.data.message) || error.message}});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin.";
          dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', error: {type: 'NetworkError', error: errorMessage}});
        }
        else {
          dispatch({
            type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
            dependencyDefinitionId,
            error: (error.response && error.response.data && error.response.data.message) || errorMessage
          });
        }
        }
    }
  },
  onAddMaterial: async (details) => {
    try {
      dispatch({type: 'ADD_COMPONENT_REQUESTED'});
      let result = await uploadStaticFiles(details);
      const response = await axios.post(apiUrl('materials'), result.jsonToPost);
      dispatch({type: 'ADD_COMPONENT_SUCCEEDED'});
      browserHistory.push(`/materials/${response.data.id}`);
      toastr.success('Success', `The Material ${response.data.id} is created successfully`);
      if (result.listOfFailedStaticFiles.length > 0) {
        toastr.warning('Failed to upload ' + result.listOfFailedStaticFiles.toString() +
          ' file(s).', 'Please retry uploading the file(s).');
      }
    } catch (error) {
      dispatch({type: 'ADD_MATERIAL_FAILED', error: error.response ? error.response.data : error.message});
      logException(error);
    }
  },

  onCancelMaterial: () => {
    browserHistory.goBack();
  },
  onAddService: async (details) => {
    try {
      dispatch({type: 'ADD_COMPONENT_REQUESTED'});
      let result = await uploadStaticFiles(details);
      const response = await axios.post(apiUrl('services'), result.jsonToPost);
      dispatch({type: 'ADD_COMPONENT_SUCCEEDED'});
      browserHistory.push(`/services/${response.data.id}`);
      toastr.success('Success', `The Service ${response.data.id} is created successfully`);
      if (result.listOfFailedStaticFiles.length > 0) {
        toastr.warning('Failed to upload ' + result.listOfFailedStaticFiles.toString() +
          ' file(s).', 'Please retry uploading the file(s).');
      }
    } catch (error) {
      dispatch({type: 'ADD_SERVICE_FAILED', error: error.response ? error.response.data : error.message});
      logException(error);
    }
  },
  onCancelService: () => {
    browserHistory.goBack();
  },
  onAssetDefinitionFetch: async (group) => {
    try {
      dispatch({type: 'ASSET_DEFINITION_FETCH_STARTED', group});
      const response = await axios.get(apiUrl(`asset-definitions/${group}`));
      const definition = response.data;
      dispatch({type: 'ASSET_DEFINITION_FETCH_SUCCEEDED', definition});
    } catch (error) {
      logException(error);
      dispatch({type: 'ASSET_DEFINITION_FETCH_FAILED', error})
    }
  },

  onCancelErrorDialog: () => {
    dispatch({type: 'CANCEL_COMPONENT_ERROR'});
  }

});

const component = connect(mapStateToProps, mapDispatchToProps)(CreateComponent);
export const CreateComponentConnector = Permissioned(component, getCreateComponentPermissions, true, null);
