import {expect} from 'chai';
import {mapStateToProps} from '../../../src/components/composite-details/connector';

describe('Connector', () => {
  describe('mapStateToProps', () => {
    it('should return the right props', () => {
      const state = { reducer: {
        search: 1,
        masterDataByName: {status: 2},
        dependencyDefinitions: 3,
        dependencyDefinitionError: 4,
        definitions: {sfg: 5},
        masterData: 6,
        components: {
        }
      }};
      const props = {
        isRequestingClassification: false,
        route: {mode: 'edit', componentType: 'sfg'},
        params: {code: 'FLR01'}, 
        routeParams: { sfgCode: 'test' }
      };
      const actual = mapStateToProps(state, props);
      const expected = {
        searchResults: 1,
        statusData: 2,
        classificationData: 3,
        classificationDataError: 4,
        isRequestingClassification: false,
        mode: 'edit',
        definition: 5,
        masterData: 6,
        code: 'FLR01',
        compositeCode: 'test',
        componentType: 'sfg',
        isSaving: false,
        sfgData: undefined,
        sfgError: undefined
      };
      expect(actual).to.eql(expected);
    });
  });
});
