import {apiUrl} from '../src/helpers';
import {EditComponent} from './components/edit-component';
import {connect} from 'react-redux';
import axios from 'axios';
import {browserHistory} from 'react-router';
import {toastr} from 'react-redux-toastr';
import {containerName, head, logException, updateColumn} from "./helpers";
import {Permissioned, permissions} from "./permissions/permissions";
import {getEditComponentPermissions} from "./permissions/ComponentPermissions";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => ({
  componentCode: props.params.componentCode,
  componentType: props.route.componentType,
  details: state.reducer.components[props.params.componentCode],
  dependencyDefinitions: state.reducer.dependencyDefinitions,
  masterData: state.reducer.masterData || {},
  componentUpdateError: state.reducer.componentUpdateError,
  componentUpdating: state.reducer.componentUpdating || false,
  assetDefinitions: state.reducer.assetDefinitions,
  error: state.reducer.dependencyDefinitionError[props.route.componentType] || state.reducer.newMaterialError
  || state.reducer.error || state.reducer.ComponentMasterDataError || false
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
            columns.push({value:null,key:column.key,name:column.name}) :
            columns.push({
            value:column.value,
            key:column.key,
            name:column.name});
          if (column.dataType.name === "StaticFile" &&
            column.value !== null) {
            if (typeof(column.value) === "number") {
              const file = window.fileList[column.value][0];
              const formData = new FormData();
              formData.append('File', file);
              staticFiles.push({
                headerKey:header.key, columnKey:column.key, formData
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
                  headerKey:header.key, columnKey:column.key, formData, index: i
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
        logException(error);
        failedArrayOfStaticFiles
          .push({headerKey: file.headerKey, index: file.index, columnKey: file.columnKey});
        listOfFailedStaticFiles.push(file.columnKey);
      }
    }));

  for (let i = 0; i < failedArrayOfStaticFiles.length; i++) {
    const staticFile = failedArrayOfStaticFiles[i];
    if (staticFile.index > -1 && head(jsonToPost,staticFile.headerKey,staticFile.columnKey,'value')) {
      let value = head(jsonToPost, staticFile.headerKey, staticFile.columnKey, 'value');
      value.splice(staticFile.index, 1);
      jsonToPost = updateColumn(jsonToPost,staticFile.headerKey,staticFile.columnKey,{value : value});
      if (value.length === 0) {
        jsonToPost = updateColumn(jsonToPost,staticFile.headerKey,staticFile.columnKey,{value : null});
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
        logException(error);
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : null});
        listOfFailedStaticFiles.push(file.columnKey);
      }
    }));

  return ({jsonToPost, listOfFailedStaticFiles});
};

export const mapDispatchToProps = (dispatch) => ({
  onMaterialFetchRequest: async (materialCode) => {
    try {
      const response = await axios.get(apiUrl(`materials/${materialCode}`) + '?dataType=true');
      logInfo('MATERIAL_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'MATERIAL_FETCH_SUCCEEDED', details: response.data});
    } catch (error) {
      logException('MATERIAL_FETCH_FAILED \n '+error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'MATERIAL_FETCH_FAILED',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'MATERIAL_FETCH_FAILED',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'MATERIAL_FETCH_FAILED', detailserror: `Material : ${materialCode} is not found`});
      }
      else {
        dispatch({type: 'MATERIAL_FETCH_FAILED', detailserror: error.message});
      }
    }
  },
  onServiceFetchRequest: async (serviceCode) => {
    try {
      const response = await axios.get(apiUrl(`services/${serviceCode}`) + "?dataType=true");
      let serviceDetails = response.data;
      const classificationDefinition = serviceDetails["classification Definition"];
      delete serviceDetails["classification Definition"];
      const details = {serviceDetails: serviceDetails, classificationDefinition: classificationDefinition};
      logInfo('SERVICE_FETCH_SUCCEEDED' + response.data);
      dispatch({type: 'SERVICE_FETCH_SUCCEEDED', details: details});
    } catch (error) {
      logException('SERVICE_FETCH_FAILED \n'+error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'SERVICE_FETCH_FAILED', error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'SERVICE_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'SERVICE_FETCH_FAILED', error: 'No Service with code: ' + serviceCode + ' is found.'});
      }
      else {
        dispatch({type: 'SERVICE_FETCH_FAILED', error: error.message});
      }
    }
  },
  onDependencyDefinitionFetch: async (dependencyDefinitionId) => {
    try {
      const response = await axios.get(apiUrl(`dependency/${dependencyDefinitionId}?compressed=true`));
      const dependencyDefinition = response.data;
      logInfo('DEPENDENCY_DEFINITION_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED', dependencyDefinitionId, dependencyDefinition});
    }
    catch (error) {
      logException('DEPENDENCY_DEFINITION_FETCH_FAILED \n' + error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error: `dependency definition: ${dependencyDefinitionId} is not found`
        });
      }
      else {
        dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', dependencyDefinitionId, error});
      }
    }
  },
  onMasterDataFetch: async (masterDataId) => {
    try {
      dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_LOADING', masterDataId});
      const response = await axios.get(apiUrl(`master-data/${masterDataId}`));
      logInfo('COMPONENT_MASTER_DATA_FETCH_SUCCEEDED\n'+response.data);
      const masterData = response.data;
      dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED', masterData});
    }
    catch (error) {
      logException('COMPONENT_MASTER_DATA_FETCH_FAILED \n' + error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'COMPONENT_MASTER_DATA_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'COMPONENT_MASTER_DATA_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error: `Master data is not found for ${masterDataId}`});
      } else {
        dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error});
      }
    }
  },
  onUpdateMaterial: async (materialId, details) => {
    try {
      dispatch({type: 'UPDATE_MATERIAL_REQUESTED'});
      let result = await uploadStaticFiles(details);
      const response = await axios.put(apiUrl(`materials/${materialId}`), result.jsonToPost);
      logInfo('UPDATE_MATERIAL_SUCCEEDED\n'+response.data);
      dispatch({type: 'UPDATE_MATERIAL_SUCCEEDED', id: response.data.id});
      browserHistory.push(`/materials/${response.data.id}`);
      toastr.success('Success', `The Material ${response.data.id} details are updated successfully`);
      if (result.listOfFailedStaticFiles.length > 0) {
        toastr.warning('Failed to upload ' + result.listOfFailedStaticFiles.toString() +
          ' file(s).', 'Please retry uploading the file(s).');
      }
    }
    catch (error) {
      logException('UPDATE_MATERIAL_FAILED\n'+error);
      dispatch({type: 'UPDATE_MATERIAL_FAILED', error: error.response ? error.response.data : error});
    }
  },
  onUpdateService: async (serviceId, details) => {
    try {
      dispatch({type: 'UPDATE_SERVICE_REQUESTED'});
      let result = await uploadStaticFiles(details);
      const response = await axios.put(apiUrl(`services/${serviceId}`), result.jsonToPost);
      logInfo('UPDATE_SERVICE_SUCCEEDED\n'+response.data);
      dispatch({type: 'UPDATE_SERVICE_SUCCEEDED', id: response.data.id});
      browserHistory.push(`/services/${serviceId}`);
      toastr.success('Success', `The Service ${response.data.id} details are updated successfully`);
      if (result.listOfFailedStaticFiles.length > 0) {
        toastr.warning('Failed to upload ' + result.listOfFailedStaticFiles.toString() +
          ' file(s).', 'Please retry uploading the file(s).');
      }
    }
    catch (error) {
      logException('UPDATE_SERVICE_FAILED\n'+error);
      dispatch({type: 'UPDATE_SERVICE_FAILED', error: error.response ? error.response.data : error});
    }
  },
  onResetError: () => {
    dispatch({type: 'RESET_UPDATE_ERROR'});
  },
  onCancelUpdateMaterial: (materialId) => {
    browserHistory.push(`/materials/${materialId}`);
  },
  onCancelUpdateService: (serviceId) => {
    browserHistory.push(`/services/${serviceId}`);
  },
  onAssetDefinitionFetch: async (group) => {
    try {
      dispatch({type: 'ASSET_DEFINITION_FETCH_STARTED', group});
      const response = await axios.get(apiUrl(`asset-definitions/${group}`));
      const definition = response.data;
      logInfo('ASSET_DEFINITION_FETCH_SUCCEEDED\n'+response.data);
      dispatch({type: 'ASSET_DEFINITION_FETCH_SUCCEEDED', definition});
    } catch (error) {
      logException('ASSET_DEFINITION_FETCH_FAILED\n'+error);
      dispatch({type: 'ASSET_DEFINITION_FETCH_FAILED', error})
    }
  }
});

const component = connect(mapStateToProps, mapDispatchToProps)(EditComponent);
export const EditComponentConnector = Permissioned(component, getEditComponentPermissions, true, null);
