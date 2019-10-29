import {addDocumentReducer} from '../../src/reducers/addDocumentReducer';
import chai, {expect} from 'chai';

describe('addDocumentReducer', () => {
  it('should return a state contains isFetching as true when action type is GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED', () => {
    const intialState = {};
    const action = {
      type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: undefined}, isFetching: true};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains data when action type is GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED', () => {
    const intialState = {};
    const action = {
      type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED',
      details: 'details'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: 'details'}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains error when action type is GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', () => {
    const intialState = {};
    const action = {
      type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED',
      error: 'error'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: 'error', data: {values: undefined}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains isFetching as true when action type is GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED', () => {
    const intialState = {};
    const action = {
      type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: undefined}, isFetching: true};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains data when action type is GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED', () => {
    const intialState = {};
    const action = {
      type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED',
      details: 'details'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: 'details'}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains error when action type is GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED', () => {
    const intialState = {};
    const action = {
      type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
      error: 'error'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: 'error', data: {values: undefined}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains isFetching as true when action type is GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED', () => {
    const intialState = {};
    const action = {
      type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: undefined}, isFetching: true};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains data when action type is GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED', () => {
    const intialState = {};
    const action = {
      type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED',
      details: 'details'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: 'details'}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains error when action type is GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED', () => {
    const intialState = {};
    const action = {
      type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED',
      error: 'error'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: 'error', data: {values: undefined}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains isFetching as true when action type is GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED', () => {
    const intialState = {};
    const action = {
      type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: undefined}, isFetching: true};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains data when action type is GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED', () => {
    const intialState = {};
    const action = {
      type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED',
      details: 'details'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: 'details'}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains error when action type is GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED', () => {
    const intialState = {};
    const action = {
      type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
      error: 'error'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: 'error', data: {values: undefined}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains no data when action type is DESTROY_MATERIAL_DOCUMENTS', () => {
    const intialState = {};
    const action = {
      type: 'DESTROY_MATERIAL_DOCUMENTS'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: undefined}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });

  it('should return a state contains no data when action type is DESTROY_SERVICE_DOCUMENTS', () => {
    const intialState = {};
    const action = {
      type: 'DESTROY_SERVICE_DOCUMENTS'
    };
    const actualState = addDocumentReducer(intialState, action);
    const expectedState = {error: '', data: {values: undefined}, isFetching: false};
    expect(actualState).to.deep.equal(expectedState);
  });
});
