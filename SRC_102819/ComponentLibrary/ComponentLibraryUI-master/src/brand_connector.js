import { connect } from 'react-redux';
import { Brand } from './components/brand';
import axios from 'axios';
import { apiUrl, uploadStaticFiles } from '../src/helpers';
import { toastr } from 'react-redux-toastr';
import {logException} from "./helpers";
import {logInfo} from "./sumologic-logger";
import {Permissioned} from "./permissions/permissions";
import {getBrandPermissions} from "./permissions/ComponentPermissions";

export const mapStateToProps = (state, props) => ({
  masterData: state.reducer.masterData || {},
  action: props.params.action,
  isSavingBrand: state.reducer.isSavingBrand || false,
  brandCode: props.params.brandCode,
  materialCode: props.params.materialCode,
  details: state.reducer.components[props.params.materialCode],
  detailsError: state.reducer.fetchBrandError,
  brandDefinition: state.reducer.brandDefinition,
  brandDefinitionError: state.reducer.brandDefinitionError,
  brandAdding: state.reducer.brandAdding,
  newBrandError: state.reducer.newBrandError,
  addedBrand: state.reducer.addedBrand || false,
  updatedBrand: state.reducer.updatedBrand || false
});

export const mapDispatchToProps = (dispatch) => ({
  onMasterDataFetch: async (masterDataId) => {
    try {
      dispatch({ type: 'COMPONENT_MASTER_DATA_FETCH_LOADING', masterDataId });
      const response = await axios.get(apiUrl(`master-data/${masterDataId}`));
      const masterData = response.data;
      logInfo('COMPONENT_MASTER_DATA_FETCH_SUCCEEDED\n'+ response.data);
      dispatch({ type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED', masterData });
    } catch (error) {
      logException('COMPONENT_MASTER_DATA_FETCH_FAILED\n'+error);
      dispatch({ type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error });
    }
  },
  onBrandFetchRequest: async (materialCode) => {
    try {
      const response = await axios.get(apiUrl(`materials/${materialCode}`) + '?dataType=true');
      const materialDetails = response.data;
      logInfo('BRAND_FETCH_SUCCEEDED\n' + response.data);
      dispatch({ type: 'BRAND_FETCH_SUCCEEDED', details: materialDetails });

    } catch (error) {
      logException('BRAND_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'BRAND_FETCH_FAILED',
          materialCode,
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'BRAND_FETCH_FAILED',
          materialCode,
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({ type: 'BRAND_FETCH_FAILED', materialCode, detailserror: 'No Brand with Material code: ' + materialCode + ' is found.' });
      }
      else {
        dispatch({ type: 'BRAND_FETCH_FAILED', materialCode, detailserror: error.message });
      }
    }
  },
  onBrandDefinitionFetchRequest: async () => {
    try {
      const response = await axios.get(apiUrl(`definitions/brands`));
      const brandDefinition = response.data;
      logInfo('BRAND_DEFINITION_FETCH_SUCCEEDED\n'+ response.data);
      dispatch({ type: 'BRAND_DEFINITION_FETCH_SUCCEEDED', brandDefinition: brandDefinition });

    } catch (error) {
      logException('BRAND_DEFINITION_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'BRAND_DEFINITION_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'BRAND_DEFINITION_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({ type: 'BRAND_DEFINITION_FETCH_FAILED', error: 'No Brand Definition is found.' });
      }
      else {
        dispatch({ type: 'BRAND_DEFINITION_FETCH_FAILED', error: error.message });
      }
    }
  },
  onAddOrEditBrand: async (materialId, details, isEdit) => {
    try {
      (isEdit ? dispatch({ type: 'EDIT_BRAND_REQUESTED' }) : dispatch({ type: 'ADD_BRAND_REQUESTED' }));
      let result = await uploadStaticFiles(details);
      const response = await axios.put(apiUrl(`materials/${materialId}`), result.jsonToPost);
      logInfo('EDIT_BRAND_SUCCEEDED\n'+response.data);
      dispatch({ type: isEdit ? 'EDIT_BRAND_SUCCEEDED' : 'ADD_BRAND_SUCCEEDED', details: response.data });
      toastr.success('success', `Brand details ${isEdit ? 'updated' : 'created'} successfully`);
      if (result.listOfFailedStaticFiles.length > 0) {
        toastr.warning('Failed to upload ' + result.listOfFailedStaticFiles.toString() +
          ' file(s).', 'Please retry uploading the file(s).');
      }
    }
    catch (error) {
      logException(isEdit ? 'EDIT_BRAND_FAILED \n' : 'ADD_BRAND_FAILED\n' + error);
      dispatch({ type: isEdit ? 'EDIT_BRAND_FAILED' : 'ADD_BRAND_FAILED', error: error.response ? error.response.data : error });
    }
  }
});

const component = connect(mapStateToProps, mapDispatchToProps)(Brand);

export const BrandConnector = Permissioned(component, getBrandPermissions, true, null);
