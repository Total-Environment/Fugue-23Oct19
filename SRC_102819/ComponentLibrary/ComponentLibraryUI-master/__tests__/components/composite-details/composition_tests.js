import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { CompositeComposition } from '../../../src/components/composite-details/composition';
import { transformCompositionDetail } from '../../../src/components/composite-details/helper';


chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('CompositeComposition', () => {
  let props = {};
  beforeEach(() => {
    props = {
      onDetailChange: sinon.spy(),
      onMakePrimaryItem: sinon.spy(),
      onRemoveCompositionItem: sinon.spy()
    }
  });

  it('should transform incoming composition details to a header column format', () => {
    const compositionDetails = [{
      headers: [
        {
          columns: [
            {
              dataType: {
                name: 'MasterData'
              },
              value: 'uom',
              name: 'Unit of Measure',
              key: 'unit_of_measure'
            }
          ],
          name: 'General',
          key: 'general'
        }
      ],
      type: 'material',
      id: 'xyz'
    }].map(c => ({ values: transformCompositionDetail(c), id: c.id }));
    const newProps = { ...props, compositionDetails, mode: 'create' };
    const wrapper = shallow(<CompositeComposition {...newProps} />);
    const rowItem = wrapper.find(`#comp_${compositionDetails[0].id}`);
    expect(rowItem).to.exist;
  });

  it('should invoke onRemoveCompositionItem on clicking remove item button', () => {
    const compositionDetails = [{
      headers: [
        {
          columns: [
            {
              dataType: {
                name: 'MasterData'
              },
              value: 'uom',
              name: 'Unit of Measure',
              key: 'unit_of_measure'
            }
          ],
          name: 'General',
          key: 'general'
        }
      ],
      type: 'material',
      id: 'xyz'
    }].map(c => ({ values: transformCompositionDetail(c), id: c.id }));

    const newProps = { ...props, compositionDetails, mode: 'create' };
    const wrapper = shallow(<CompositeComposition {...newProps} />);
    const rowItem = wrapper.find(`#comp_${compositionDetails[0].id}`);
    rowItem.find('a').simulate('click', { preventDefault: sinon.spy() });
    expect(newProps.onRemoveCompositionItem).to.be.calledWith(compositionDetails[0].id);
  });

  it('should render "Make Primary" checkbox for only service resource type', () => {
    const compositionDetails = [{
      headers: [
        {
          columns: [
            {
              dataType: {
                name: 'MasterData'
              },
              value: 'uom',
              name: 'Unit of Measure',
              key: 'unit_of_measure'
            }
          ],
          name: 'General',
          key: 'general'
        },
        {

        }
      ],
      type: 'material',
      id: 'xyz'
    }, {
      headers: [
        {
          columns: [
            {
              dataType: {
                name: 'MasterData'
              },
              value: 'uom',
              name: 'Unit of Measure',
              key: 'unit_of_measure'
            }
          ],
          name: 'General',
          key: 'general'
        },
        {

        }
      ],
      type: 'service',
      id: 'xyz2'
    }].map(c => ({ values: transformCompositionDetail(c), id: c.id }));

    const newProps = { ...props, compositionDetails, mode: 'create', hasPrimary: true, componentType: 'sfg' };
    const wrapper = shallow(<CompositeComposition {...newProps} />);
    const materialRow = wrapper.find(`#comp_${compositionDetails[0].id}`);
    const serviceRow = wrapper.find(`#comp_${compositionDetails[1].id}`);
    expect(materialRow.find('input[type="checkbox"]')).to.not.exist;
    expect(serviceRow.find('input[type="checkbox"]')).to.exist;
    serviceRow.find('input[type="checkbox"]').simulate('change');
    expect(newProps.onMakePrimaryItem).to.be.calledWith(compositionDetails[1].id);
  });
});
