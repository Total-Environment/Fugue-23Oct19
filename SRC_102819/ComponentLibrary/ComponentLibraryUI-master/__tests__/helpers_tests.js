import {paginationConnector, __RewireAPI__ as HelperRewired} from "../src/helpers";
import sinon from "sinon";
import {expect} from 'chai';
import {head, col} from '../src/helpers';

describe('Helpers', () => {
  describe('column', () => {
    let columnDetails, headerDetails;
    beforeEach(() => {
      columnDetails = {
        "dataType": {
          "name": "Constant",
          "subType": "Primary"
        },
        "key": "material_level_1",
        "name": "Material Level 1",
        "value": "Primary"
      };
      headerDetails = {
        "columns": [
          columnDetails
        ],
        "key": "classification",
        "name": "Classification"
      };
    });
    it('should return column when called with header and column', () => {
      expect(col(headerDetails, 'material_level_1')).to.eql(columnDetails);
    });
    it('should get any subsequent calls in Ramda path syntax', () => {
      expect(col(headerDetails, 'material_level_1', 'dataType', 'name')).to.eql('Constant');
    })
  });
  describe('header', () => {
    let details, headerDetails, columnDetails;
    beforeEach(() => {
      columnDetails = {
        "dataType": {
          "name": "Constant",
          "subType": "Primary"
        },
        "key": "material_level_1",
        "name": "Material Level 1",
        "value": "Primary"
      };
      headerDetails = {
        "columns": [
          columnDetails
        ],
        "key": "classification",
        "name": "Classification"
      };
      details = {"headers": [headerDetails]};
    });
    it('should return undefined if data is undefined', () => {
      expect(head(undefined, 'classification')).to.equal(undefined);
    });
    it('should return details if there is no selector', () => {
      expect(head(details)).to.eql(details);
    });
    it('should return header when called with header', () => {
      expect(head(details, 'classification')).to.eql(headerDetails);
    });
  });
//   describe('paginationConnector', () => {
//     it('it returns a function that returns an object with onAmendQueryParams when called with a function', () => {
//       const dispatchSpy = sinon.spy();
//       const ownProps = {};
//       const returnFn = paginationConnector(sinon.stub(dispatchSpy, ownProps).returns({a: 1}));
//       const connector = returnFn(dispatchSpy);
//       expect(connector).to.have.property('a');
//       expect(connector).to.have.property('onAmendQueryParams');
//     });
//
//     describe('onAmendQueryParams', () => {
//       it('should call browserHistory push with the current url and new Params', () => {
//         const dispatchSpy = sinon.spy();
//         const browserPushSpy = sinon.spy();
//         HelperRewired.__Rewire__('browserHistory', {push: browserPushSpy});
//         const ownProps = {
//           location: {
//             pathname: '/materials/MCH022854/rental-rate-history',
//             query: {
//               pageNumber: 1,
//             }
//           }
//         };
//         const {onAmendQueryParams} = paginationConnector(sinon.stub().returns({}))(dispatchSpy, ownProps);
//         onAmendQueryParams({pageNumber: 2});
//
//         expect(browserPushSpy).to.have.been.calledWith(`/${ownProps.pathname}?pageNumber=2`);
//       });
//     });
//   });
});
