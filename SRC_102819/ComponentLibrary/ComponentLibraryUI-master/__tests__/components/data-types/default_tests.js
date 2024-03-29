import React from 'react';
import { shallow } from 'enzyme';
import chai, { expect } from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import { Default } from '../../../src/components/data-types/default';
import sinon from 'sinon';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Default', () => {
  describe('render', () => {
    let props;
    beforeEach(() => {
      props = {columnName: "Maintain lot", columnValue:  {
        "value": "Maintain lot"}};
    });
    it('should render header and column value', () => {
      const wrapper = shallow(<Default {...props}/>);
      expect(wrapper).to.have.text('Maintain lot');
    });

    context('editable is true', () => {
      it('should render input', () => {
        props.columnValue.editable = true;
        const wrapper = shallow(<Default {...props}/>);
        expect(wrapper).to.have.descendants('input');
      });
    });

    it('should call on change with proper value and when no data type is provided', ()=>{
      let props = {columnName: "Sample",
        columnValue:{
          value: null,
          editable: true,
          dataType: {
            type:"Default",
            subType: null
          },
        },
        onChange: sinon.spy()
      };
      const wrapper = shallow(<Default {...props}/>);
      expect(wrapper).to.have.descendants('input');
      wrapper.find('input').simulate('change', {target: { value: 'string type'} });
      expect(props.onChange).to.have.been.calledWith('string type');
    });
  })
});
