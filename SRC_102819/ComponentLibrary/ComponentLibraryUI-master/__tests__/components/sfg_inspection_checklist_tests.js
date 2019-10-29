import {SFGInspectionChecklist} from '../../src/components/checklists/sfg_inspection_checklist';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import React from 'react';
import {shallow} from 'enzyme';

chai.use(chaiEnzyme());

describe('sfg inspection checklist', () => {
  let props;
  let wrapper;

  beforeEach(() => {
    props = {
      details: {
        checkListId: 'SSTQ001',
        id: '23',
        template: 'sfginspectionchecklist',
        title: 'sfg inspection checklist - Aluminium Products',
        content: {
          entries: [{
            cells: [{
              key: '0 0',
              value: 'S.No'
            }]
          },
            {
              cells: [{
                key: '1 0',
                value: '1'
              }]
            }]
        }
      },
    };
    wrapper = shallow(<SFGInspectionChecklist {...props}/>);
  });

  it('should render error message if entries in details are not found', () => {
    props.details.content.entries = null;
    wrapper = shallow(<SFGInspectionChecklist {...props}/>);
    const result = wrapper.find('div');
    expect(result.contains('sfg inspection checklist data is not found')).to.equal(true);
  });

  it('should render header data', () => {
    expect(wrapper).to.contain(<b>Project Name:</b>);
    expect(wrapper).to.contain(<b>sfg inspection checklist - Aluminium Products</b>);
    expect(wrapper).to.contain(<b>SSTQ001</b>);
    expect(wrapper).to.contain(<b>TE/</b>);
  });

  it('should render Content, Remarks, Footer', () => {
    expect(wrapper).to.have.descendants('Content');
    const result = wrapper.find('Content');
    expect(result).to.have.prop('entries').deep.equal(props.details.content.entries);
    expect(wrapper).to.have.descendants('RemarksWithoutInstruments');
    expect(wrapper).to.have.descendants('SFGFooter');
  });
});
