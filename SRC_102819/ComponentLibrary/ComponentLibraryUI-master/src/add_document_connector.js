import { connect } from 'react-redux';
import { AddDocumentDialog } from './components/add-document-dialog';
import { apiUrl } from '../src/helpers';
import axios from 'axios';
import { logException } from "./helpers";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => {
  return {
    componentType: props.componentType,
    group: props.group,
    columnName: props.columnName,
    columnHeader: props.columnHeader,
    showLocalSystem: props.showLocalSystem,
    data: state.addDocumentReducer.data,
    error: state.addDocumentReducer.error,
    isFetching: state.addDocumentReducer.isFetching || false,
    onOk: props.onOk,
    onCancel: props.onCancel
  }
};

export const mapDispatchToProps = (dispatch) => ({
  getSFGDocumentsByGroupAndColumnName: async (group, columnName, pageNumber, keywords) => {
    try {
      dispatch({ type: 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_REQUESTED' });
      const response = await axios.get(apiUrl('sfgs/documents'), { params: { group, columnName, keywords, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      logInfo('GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_SUCCEEDED\n'+response.data);
      dispatch({ type: 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException('GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED\n' + error);
      if (error.response && error.response.status === 400) {
        dispatch({ type: 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED', error: { type: 'BadRequest' } });
      } else {
        dispatch({ type: 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED', error: { type: 'Unknown', error } });
      }
    }
  },
  getPackageDocumentsByGroupAndColumnName: async (group, columnName, pageNumber, keywords) => {
    try {
      dispatch({ type: 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_REQUESTED' });
      const response = await axios.get(apiUrl('packages/documents'), { params: { group, columnName, keywords, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      logInfo('GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_SUCCEEDED\n'+response.data);
      dispatch({ type: 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException('GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED\n'+error);
      if (error.response && error.response.status === 400) {
        dispatch({ type: 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED', error: { type: 'BadRequest' } });
      } else {
        dispatch({ type: 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED', error: { type: 'Unknown', error } });
      }
    }
  },
  getMaterialDocumentsByGroupAndColumnName: async (materialGroup, columnName, pageNumber) => {
    try {
      dispatch({ type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED' });
      const response = await axios.get(apiUrl('materials/documents'), { params: { materialGroup, columnName, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      logInfo('GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED\n'+response.data);
      dispatch({ type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException('GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED\n'+error);
      if (error.response && error.response.status === 400) {
        dispatch({ type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', error: { type: 'BadRequest' } });
      } else {
        dispatch({ type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', error: { type: 'Unknown', error } });
      }
    }
  },
  getMaterialDocumentsByGroupAndColumnNameAndKeyWord: async (materialGroup, columnName, keywords, pageNumber) => {
    try {
      dispatch({ type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED' });
      const response = await axios.get(apiUrl('materials/documents'), { params: { materialGroup, columnName, keywords, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      dispatch({ type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException(error);
      if (error.response && error.response.status === 400) {
        dispatch({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'BadRequest' }
        });
      } else {
        dispatch({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'Unknown', error }
        });
      }
    }
  },
  getServiceDocumentsByGroupAndColumnName: async (group, columnName, pageNumber) => {
    try {
      dispatch({ type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED' });
      const response = await axios.get(apiUrl('services'), { params: { group, columnName, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      dispatch({ type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException(error);
      if (error.response && error.response.status === 400) {
        dispatch({ type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', error: { type: 'BadRequest' } });
      } else {
        dispatch({ type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', error: { type: 'Unknown', error } });
      }
    }
  },
  getServiceDocumentsByGroupAndColumnNameAndKeyWord: async (group, columnKey, keyWord, pageNumber) => {
    try {
      dispatch({ type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED' });
      const response = await axios.get(apiUrl('services'), { params: { group, columnKey, keyWord, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      dispatch({ type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException(error);
      if (error.response && error.response.status === 400) {
        dispatch({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'BadRequest' }
        });
      } else {
        dispatch({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'Unknown', error }
        });
      }
    }
  },
  getBrandDocumentsByGroupAndColumnName: async (group, brandColumnName, pageNumber) => {
    try {
      dispatch({ type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED' });
      const response = await axios.get(apiUrl('materials/brands/documents'), { params: { materialGroup: group, brandColumnName, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      dispatch({ type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException(error);
      if (error.response && error.response.status === 400) {
        dispatch({ type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', error: { type: 'BadRequest' } });
      } else {
        dispatch({ type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', error: { type: 'Unknown', error } });
      }
    }
  },
  getBrandDocumentsByGroupAndColumnNameAndKeyWord: async (group, brandColumnName, keywords, pageNumber) => {
    try {
      dispatch({ type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED' });
      const response = await axios.get(apiUrl('materials/brands/documents'), { params: { materialGroup: group, brandColumnName, keywords, pageNumber, batchSize: 10 } });
      const responseJson = response.data;
      dispatch({ type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED', details: responseJson });
    }
    catch (error) {
      logException(error);
      if (error.response && error.response.status === 400) {
        dispatch({
          type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'BadRequest' }
        });
      } else {
        dispatch({
          type: 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'Unknown', error }
        });
      }
    }
  },
  destroyMaterialDocuments() {
    dispatch({ type: 'DESTROY_MATERIAL_DOCUMENTS' });
  },
  destroyServiceDocuments() {
    dispatch({ type: 'DESTROY_SERVICE_DOCUMENTS' });
  },
  destroyBrandDocuments() {
    dispatch({ type: 'DESTROY_BRAND_DOCUMENTS' });
  },
  destroySFGDocuments() {
    dispatch({ type: 'DESTROY_SFG_DOCUMENTS' });
  },
  destroyPackageDocuments() {
    dispatch({ type: 'DESTROY_PACKAGE_DOCUMENTS' });
  }
});

export const AddDocumentConnector = connect(mapStateToProps, mapDispatchToProps)(AddDocumentDialog);
