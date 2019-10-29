import {expect} from 'chai';
import {mapStateToProps} from '../../../src/components/price-book/connector';

describe('PriceBook Connector', () => {
  describe('mapStateToProps', () => {
    it('should take cprs from state', () => {
      const state = {reducer: {cprs: 1, dependencyDefinitions: 2, projects: 3}};
      const result = mapStateToProps(state);
      expect(result).to.eql({cprs: 1, dependencyDefinitions: 2, projects: 3, dependencyDefinitionError: undefined,
        cprError: undefined,
        projectsError: undefined});
    });
  });
});
