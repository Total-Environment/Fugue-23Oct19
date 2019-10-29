import chai, {expect} from 'chai';
import {
  mapDispatchToProps,
  mapStateToProps,
  __RewireAPI__ as ConnectorRewired
} from '../src/component_search_connector';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import {apiUrl} from "../src/helpers";

chai.use(sinonChai);

describe('Component Search Connector', () => {
  describe('mapStateToProps', () => {
    context('it is not listing', () => {
      let state, props;
      beforeEach(() => {
        state = {
          reducer: {
            search: 1,
            error: '',
            isFiltering: false,
            filterData: []
          }
        };
        props = {
          route: {componentType: 'material'},
          params: {group: 'Clay Material', keyword: 'sattar'},
          location: {
            query: {
              sortOrder: 'Ascending',
              sortColumn: 'material Code',
              selectedTab: 'General',
              pageNumber: '1',
            },
          },
        };
      });
      it('should send the right props when it is not listing', () => {
        const expected = {
          componentType: 'material',
          search: 1,
          group: 'Clay Material',
          keyword: 'sattar',
          error: '',
          sortOrder: 'Ascending',
          sortColumn: 'material Code',
          selectedTab: 'General',
          pageNumber: 1,
          isFiltering: false,
          filterData: []
        };
        expect(mapStateToProps(state, props)).to.eql(expected);
      });

      it('should send componentType as SFG if url prop is sfg', () => {
        props.route.componentType = 'sfg';
        expect(mapStateToProps(state, props).componentType).to.eql('SFG');
      });
      it('should send empty filterData if it is empty', () => {
        state.reducer.filterData = undefined;
        expect(mapStateToProps(state, props).filterData).to.eql([]);
      });
      it('should send pageNumber as 1 if it does not exist', () => {
        props.location.query.pageNumber = undefined;
        expect(mapStateToProps(state, props).pageNumber).to.eql(1);
      });
    });
    context('it is listing', () => {
      let props, state;
      beforeEach(() => {
        props = {componentType: 'material', isListing: true};
        state = {
          reducer: {
            search: 1,
            error: 'error',
          }
        };
      });
      it('should send the right props', () => {
        const expected = {
          componentType: 'material',
          search: 1,
          group: '',
          keyword: '',
          error: 'error',
        };
        expect(mapStateToProps(state, props)).to.eql(expected);
      });
    });
  });

  describe('mapDispatchToProps', () => {
    let dispatchSpy, ownProps, callbacks;
    beforeEach(() => {
      dispatchSpy = sinon.spy();
      ownProps = {
        params: {group: 'Clay Material', keyword: 'sattar'},
        location: {
          query: {},
        },
        route: {componentType: 'material'}
      };
      callbacks = mapDispatchToProps(dispatchSpy, ownProps);
    });
    describe('onMaterialSearchResultsFetch', () => {
      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is listing', async () => {
        const axiosGetStub = sinon.stub();
        axiosGetStub
          .withArgs(apiUrl('materials/all'), {params: {pageNumber: 1, details: true, batchSize: 20}})
          .returns(Promise.resolve({data: {items: [1, 2]}}));
        ConnectorRewired.__Rewire__('axios', {get: axiosGetStub});

        await callbacks.onMaterialSearchResultsFetch('Clay Material', 'gen', 1, true, 'material_code', 'material', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_STARTED',
          isFiltering: false,
          filterData: []
        });
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is not listing', async () => {
        const axiosPostStub = sinon.stub();
        axiosPostStub
          .withArgs(apiUrl('materials/searchwithingroup'), {
            groupname: 'Clay Material',
            searchQuery: 'gen',
            pageNumber: 1,
            batchSize: 20,
            details: true,
            sortColumn: 'material_code',
            sortOrder: 'Ascending',
            filterDatas: []
          })
          .returns(Promise.resolve({data: {items: [1, 2]}}));
        ConnectorRewired.__Rewire__('axios', {post: axiosPostStub});

        await callbacks.onMaterialSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_FAILED with NotFound when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {status: 404}}))});
        await callbacks.onMaterialSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with BadRequest when status code is 400', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {status: 400}}))});
        await callbacks.onMaterialSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with Unknown when status code is something else', async () => {
        ConnectorRewired.__Rewire__('axios', {
          post: sinon.stub().returns(Promise.reject({
            response: {
              status: 7070,
              data: {message: 'Message'}
            }
          }))
        });
        await callbacks.onMaterialSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_FAILED',
          error: {type: 'Unknown', error: 'Message'}
        });
      });
    });
    describe('onMaterialSearchDestroy', () => {
      it('should dispatch SEARCH_RESULT_DESTROYED', () => {
        callbacks.onMaterialSearchDestroy();
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_DESTROYED'});
      });
    });
    describe('onServiceSearchResultsFetch', () => {
      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with if it is listing', async () => {
        const axiosGetStub = sinon.stub();
        axiosGetStub
          .withArgs(apiUrl('services'), {params: {pageNumber: 1, details: true, batchSize: 20}})
          .returns(Promise.resolve({data: {items: [1, 2]}}));
        ConnectorRewired.__Rewire__('axios', {get: axiosGetStub});

        await callbacks.onServiceSearchResultsFetch('Clay Material', 'gen', 1, true, 'material_code', 'material', []);
        expect(dispatchSpy).to.have.been.calledWith({ type: 'SEARCH_RESULT_DESTROYED' });
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_STARTED',
          isFiltering: false,
          filterData: []
        });
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is not listing', async () => {
        const axiosPostStub = sinon.stub();
        axiosPostStub
          .withArgs(apiUrl('services/searchwithingroup'), {
            groupname: 'Flooring',
            searchQuery: 'gen',
            pageNumber: 1,
            batchSize: 20,
            details: true,
            sortColumn: 'service_code',
            sortOrder: 'Ascending',
            filterDatas: []
          })
          .returns(Promise.resolve({data: {items: [1, 2]}}));
        ConnectorRewired.__Rewire__('axios', {post: axiosPostStub});

        await callbacks.onServiceSearchResultsFetch('Flooring', 'gen', 1, false, 'service_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_FAILED with NotFound when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {status: 404}}))});
        await callbacks.onServiceSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with BadRequest when status code is 400', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {status: 400}}))});
        await callbacks.onServiceSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with Unknown when status code is something else', async () => {
        ConnectorRewired.__Rewire__('axios', {
          post: sinon.stub().returns(Promise.reject({
            response: {
              status: 7070,
              data: {message: 'Message'}
            }
          }))
        });
        await callbacks.onServiceSearchResultsFetch('Clay Material', 'gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_FAILED',
          error: {type: 'Unknown', error: 'Message'}
        });
      });
    });
    describe('onServiceSearchDestroy', () => {
      it('should dispatch SEARCH_RESULT_DESTROYED', () => {
        callbacks.onServiceSearchDestroy();
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_DESTROYED'});
      });
    });
    describe('onSfgSearchResultsFetch', () => {
      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is listing', async () => {
        const axiosGetStub = sinon.stub();
        axiosGetStub
          .withArgs(apiUrl('sfgs/all'), {params: {pageNumber: 1, details: true, batchSize: 20}})
          .returns(Promise.resolve({data: {items: [1, 2]}}));
        ConnectorRewired.__Rewire__('axios', {get: axiosGetStub});

        await callbacks.onSfgSearchResultsFetch('gen', 1, true, 'material_code', 'material', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_STARTED',
          isFiltering: false,
          filterData: []
        });
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is not listing', async () => {
        const axiosPostStub = sinon.stub();
        axiosPostStub
          .withArgs(apiUrl('sfgs/search'), {
            searchQuery: 'gen',
            pageNumber: 1,
            batchSize: 20,
            details: true,
            sortColumn: 'material_code',
            sortOrder: 'Ascending',
            filterDatas: []
          })
          .returns(Promise.resolve({data: {items: [1, 2]}}));
        ConnectorRewired.__Rewire__('axios', {post: axiosPostStub});

        await callbacks.onSfgSearchResultsFetch('gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_FAILED with NotFound when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {status: 404}}))});
        await callbacks.onSfgSearchResultsFetch('gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with BadRequest when status code is 400', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {status: 400}}))});
        await callbacks.onSfgSearchResultsFetch('gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with Unknown when status code is something else', async () => {
        ConnectorRewired.__Rewire__('axios', {
          post: sinon.stub().returns(Promise.reject({
            response: {
              status: 7070,
              data: {message: 'Message'}
            }
          }))
        });
        await callbacks.onSfgSearchResultsFetch('gen', 1, false, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_FAILED',
          error: {type: 'Unknown', error: 'Message'}
        });
      });
    });


    describe('onPackageSearchResultsFetch', () => {
      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is listing', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: {items: [1, 2]}}))});

        await callbacks.onPackageSearchResultsFetch('gen', 1, true, 'pkg_code', 'pkg', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_STARTED',
          isFiltering: false,
          filterData: []
        });

        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_SUCCEEDED with transformed data if it is not listing', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.resolve({data: {items: [1, 2]}}))});

        await callbacks.onPackageSearchResultsFetch('gen', 1, false, 'pkg_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_SUCCEEDED',
          details: {items: [1, 2]}
        });
      });

      it('should dispatch SEARCH_RESULT_FETCH_FAILED with NotFound when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response: {status: 404}}))});
        await callbacks.onPackageSearchResultsFetch('gen', 1, true, 'pkg_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with BadRequest when status code is 400', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response: {status: 400}}))});
        await callbacks.onPackageSearchResultsFetch('gen', 1, true, 'pkg_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      });
      it('should dispatch SEARCH_RESULT_FETCH_FAILED with Unknown when status code is something else', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub().returns(Promise.reject({
            response: {
              status: 7070,
              data: {message: 'Message'}
            }
          }))
        });
        await callbacks.onPackageSearchResultsFetch('gen', 1, true, 'material_code', 'Ascending', []);
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'SEARCH_RESULT_FETCH_FAILED',
          error: {type: 'Unknown', error: 'Message'}
        });
      });
    });
    describe('onPackageSearchDestroy', () => {
      it('should dispatch SEARCH_RESULT_DESTROYED', () => {
        callbacks.onPackageSearchDestroy();
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_DESTROYED'});
      });
    });


    describe('onSfgSearchDestroy', () => {
      it('should dispatch SEARCH_RESULT_DESTROYED', () => {
        callbacks.onSfgSearchDestroy();
        expect(dispatchSpy).to.have.been.calledWith({type: 'SEARCH_RESULT_DESTROYED'});
      });
    });
    describe('onAmendQueryParams', () => {
      it('should call browserHistory push with new params in the URL', () => {
        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        callbacks.onAmendQueryParams({a: 1});
        expect(pushSpy).to.have.been.calledWith('/materials/search/Clay Material/sattar?a=1');
      });
      it('should call browserHistory push with new SFG URL when propType is SFG', () => {
        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        ownProps.route.componentType = 'sfg';
        callbacks = mapDispatchToProps(dispatchSpy, ownProps);
        callbacks.onAmendQueryParams({a: 1});
        expect(pushSpy).to.have.been.calledWith('/sfgs/search/sattar?a=1');
      });
    });
  });
});
